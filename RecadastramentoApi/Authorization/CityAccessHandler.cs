using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

using RecadastramentoApi.Authentication;

namespace RecadastramentoApi.Authorization;

public sealed class CityAccessHandler : AuthorizationHandler<CityAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CityAccessRequirement requirement)
    {
        var httpContext = context.Resource switch
        {
            HttpContext currentHttpContext => currentHttpContext,
            AuthorizationFilterContext filterContext => filterContext.HttpContext,
            _ => null
        };

        if (httpContext is null)
        {
            return Task.CompletedTask;
        }

        var requestedCity = httpContext.Request.RouteValues["cidade"]?.ToString();
        if (string.IsNullOrWhiteSpace(requestedCity))
        {
            return Task.CompletedTask;
        }

        var clientCity = context.User.FindFirst(AuthenticationConstants.ClientCityClaim)?.Value;

        if (!string.IsNullOrWhiteSpace(clientCity) &&
            string.Equals(clientCity, requestedCity, StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
