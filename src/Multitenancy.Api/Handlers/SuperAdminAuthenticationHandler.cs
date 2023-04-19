using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Multitenancy.Common.Constants;
using Multitenancy.Services.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Multitenancy.Api.Handlers;

class AdminAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AdminAuthOptions _adminAuthOptions;

    public AdminAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        IOptions<AdminAuthOptions> adminAuthOptions,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
        _adminAuthOptions = adminAuthOptions.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(ApplicationHeaders.AdminFlowKey))
        {
            return AuthenticateResult.Fail($"{ApplicationHeaders.AdminFlowKey} header is missing");
        }

        var apiKeyToValidate = Request.Headers[ApplicationHeaders.AdminFlowKey];

        if (apiKeyToValidate == _adminAuthOptions.ApiKeyValue)
        {
            return AuthenticateResult.Success(CreateTicket());
        }

        return AuthenticateResult.Fail("Invalid API key");
    }

    private AuthenticationTicket CreateTicket()
    {
        var identity = new ClaimsIdentity(new List<Claim>(), Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return ticket;
    }
}