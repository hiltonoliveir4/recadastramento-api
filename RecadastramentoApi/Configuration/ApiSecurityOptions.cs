namespace RecadastramentoApi.Configuration;

public sealed class ApiSecurityOptions
{
    public const string SectionName = "ApiSecurity";

    public List<ApiClientOptions> Clients { get; set; } = [];
}

public sealed class ApiClientOptions
{
    public string ClientId { get; set; } = string.Empty;

    public string ApiKeyHash { get; set; } = string.Empty;

    public string ApiKeySalt { get; set; } = string.Empty;

    public int Iterations { get; set; } = 120000;

    public string City { get; set; } = string.Empty;
}
