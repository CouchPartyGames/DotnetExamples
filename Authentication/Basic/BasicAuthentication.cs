#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Example of Basic Authentication
//
// https://en.wikipedia.org/wiki/Basic_access_authentication
// HTTP Basic Authentication uses a Header that passes credentials to the server for validation
// Authorization: Basic <credentials>
// <credentials> is a username + ":" + password that is base64 encoded
//
// Use HTTPS to avoid credentials being passed insecurely
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddBasic(BasicAuthenticationDefaults.AuthenticationScheme, opts =>
    {
        opts.Name = "hello";
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/user", () => "Hello World!")
    .RequireAuthorization();
app.Run();


// Step - Create Authentication Defaults 
//  This is not a required step but highly recommended to avoid magic strings
public static class BasicAuthenticationDefaults
{
    public const string AuthenticationScheme = "Basic";
}

// Step - Extensions that allow for easy injection of Basic Authentication
public static class BasicAuthenticationExtensions
{
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder) => 
        builder.AddBasic(BasicAuthenticationDefaults.AuthenticationScheme, null, _  => {});
    
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder,
        Action<BasicAuthenticationOptions> options) =>
            builder.AddBasic(BasicAuthenticationDefaults.AuthenticationScheme, null, options);
    
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string? authenticationScheme, 
        Action<BasicAuthenticationOptions>? options) => 
            builder.AddBasic(BasicAuthenticationDefaults.AuthenticationScheme, null, options);

    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme, 
        string? displayName, 
        Action<BasicAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(authenticationScheme, displayName,
            configureOptions);
    }
}

// Step - Allow for Customization of the Handler
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions?view=aspnetcore-9.0
public sealed class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Name { get; set; }
}

// Step - Create a Handler
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationhandler-1?view=aspnetcore-9.0
public sealed class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    public const string AuthorizationHeaderName = "Authorization";
    
    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }
    
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }
        
        string header = Request.Headers[AuthorizationHeaderName];
        if (string.IsNullOrEmpty(header))
        {
            return AuthenticateResult.Fail("Unknown Authorization Format");
        }

        if (!header.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Invalid Basic Authentication Header");
        }

        var token = header.Substring(6);
        var credsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var creds = credsString.Split(":");
        if (creds.Length != 2)
        {
            return AuthenticateResult.Fail("Invalid Basic Credentials Format");
        }
        
        
        var schemeName = Scheme.Name ?? BasicAuthenticationDefaults.AuthenticationScheme;
        List<Claim> claims = [new Claim(ClaimTypes.NameIdentifier, "Bob"), new Claim(ClaimTypes.Role, "Admin")];
        ClaimsIdentity identity = new(claims, name);
        ClaimsPrincipal principal = new(identity);

        var ticket = new AuthenticationTicket(principal, schemeName);
        return AuthenticateResult.Success(ticket);
    }
}