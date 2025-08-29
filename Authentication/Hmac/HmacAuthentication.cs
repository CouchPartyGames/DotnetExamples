#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddHmac();
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();


public static class HmacAuthenticationDefaults
{
    public const string AuthenticationScheme = "Hmac";
}

public static class HmacAuthenticationExtensions
{
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder) => 
        builder.AddHmac(HmacAuthenticationDefaults.AuthenticationScheme, null, _  => {});
    
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder,
        Action<HmacAuthenticationOptions> options) =>
        builder.AddHmac(HmacAuthenticationDefaults.AuthenticationScheme, null, options);
    
    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder, string? authenticationScheme, 
        Action<HmacAuthenticationOptions>? options) => 
        builder.AddHmac(HmacAuthenticationDefaults.AuthenticationScheme, null, options);

    public static AuthenticationBuilder AddHmac(this AuthenticationBuilder builder, string authenticationScheme, 
        string? displayName, 
        Action<HmacAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(authenticationScheme, displayName,
            configureOptions);
    }
}

// Step - Allow for Customization of the Handler
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions?view=aspnetcore-9.0
public sealed class HmacAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Name { get; set; }
}

public sealed class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
{
    public HmacAuthenticationHandler(IOptionsMonitor<HmacAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }
    
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var scheme = "";
        var principal = new ClaimsPrincipal(
        [ 
            new ClaimsIdentity([
                new Claim("Name", "Test")
            ], scheme)
        ]);
        var ticket = new AuthenticationTicket(principal, scheme);
        return AuthenticateResult.Success(ticket);
    }
}
