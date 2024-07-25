using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Calendar.Authentication;
using Calendar.General.BasicAuthorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calendar.Service.Filters;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.Yield();

        // skip authentication if endpoint has [UserBasicAuthAttribute] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<UserBasicAuthAttribute>() == null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        IUser user;
        try
        {
            user = ExtractUserNameAndPassword();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Invalid Authorization Header");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        if (user == null)
            return AuthenticateResult.Fail("Invalid Username or Password");

        var principal = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity(user.Identity), user.Roles));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private IUser ExtractUserNameAndPassword()
    {
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
        var username = credentials[0];
        var password = credentials[1];

        var repository = new ServiceUserRepository();
        return repository.Authenticate(username, password, out _);
    }
}