using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DeveloperEvaluation.Api.Authentication;

public class FakeAuthHandler : AuthenticationHandler<FakeAuthOptions>
{
    public FakeAuthHandler(
        IOptionsMonitor<FakeAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "fake-auth"),
            new Claim(ClaimTypes.Name, "Fake Auth"),
            new Claim(ClaimTypes.Role, "ADM"),
            new Claim("userId", "fake-auth"),
            new Claim("username", "fake-auth"),
            new Claim("role", "ADM"),
        }, "Fake");

        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "Fake")));
    }
}

public class FakeAuthOptions : AuthenticationSchemeOptions
{
}
