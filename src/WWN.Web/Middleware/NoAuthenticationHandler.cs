using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace WWN.Web.Middleware;

public class NoAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public NoAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var principal = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(Array.Empty<System.Security.Claims.Claim>(), "NoAuth"));
        var ticket = new AuthenticationTicket(principal, "NoAuth");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
