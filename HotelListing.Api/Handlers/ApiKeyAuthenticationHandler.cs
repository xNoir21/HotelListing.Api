using System.Security.Claims;
using System.Text.Encodings.Web;
using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HotelListing.Api.Handlers;

public class ApiKeyAuthenticationHandler (
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyValidatorService validator
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey = string.Empty;

        if (Request.Headers.TryGetValue(AuthenticationDefaults.ApiKeyHeaderName, out var apiKeyHeaderValue))
        {
            apiKey = apiKeyHeaderValue.ToString();
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.NoResult();
        }
        
        
        var keyParts = apiKey["xnoir_".Length..].Split(".");
        
        var valid = await validator.IsApiKeyValid(keyParts[0], keyParts[1], Context.RequestAborted);
        if (!valid)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "apikey"),
            new(ClaimTypes.Name, "ApiKeyClient")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}