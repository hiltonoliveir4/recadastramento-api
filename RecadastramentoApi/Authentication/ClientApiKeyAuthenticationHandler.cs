using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using RecadastramentoApi.Configuration;

namespace RecadastramentoApi.Authentication;

public sealed class ClientApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<ApiSecurityOptions> securityOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly ApiSecurityOptions _securityOptions = securityOptions.Value;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthenticationConstants.ClientIdHeader, out var clientIdValues) ||
            !Request.Headers.TryGetValue(AuthenticationConstants.ApiKeyHeader, out var apiKeyValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing authentication headers."));
        }

        var clientId = clientIdValues.ToString();
        var apiKey = apiKeyValues.ToString();

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication headers are empty."));
        }

        var client = _securityOptions.Clients.FirstOrDefault(item =>
            string.Equals(item.ClientId, clientId, StringComparison.OrdinalIgnoreCase));

        if (client is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Client not found."));
        }

        if (string.IsNullOrWhiteSpace(client.City))
        {
            return Task.FromResult(AuthenticateResult.Fail("Client city is not configured."));
        }

        if (!TryDecodeBase64(client.ApiKeySalt, out var salt) || !TryDecodeBase64(client.ApiKeyHash, out var expectedHash))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API security configuration."));
        }

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            apiKey,
            salt,
            Math.Max(client.Iterations, 10000),
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        if (!CryptographicOperations.FixedTimeEquals(expectedHash, computedHash))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, client.ClientId),
            new(ClaimTypes.Name, client.ClientId),
            new(AuthenticationConstants.ClientCityClaim, client.City)
        };

        var identity = new ClaimsIdentity(claims, AuthenticationConstants.SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationConstants.SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static bool TryDecodeBase64(string input, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(input);
            return true;
        }
        catch (FormatException)
        {
            bytes = [];
            return false;
        }
    }
}
