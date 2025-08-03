#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web


// https://github.com/loresoft/AspNetCore.SecurityKey
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddCustom();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();


public static class CustomAuthenticationDefaults
{
    public const string AuthenticationScheme = "Custom";
}

public static class AuthenticationBuilderExtensions {
    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder,
        Action<CustomAuthenticationOptions>? options, string? authenticationScheme)
    {
        var scheme = authenticationScheme ?? CustomAuthenticationDefaults.AuthenticationScheme;
        var displayName = authenticationScheme ?? CustomAuthenticationDefaults.AuthenticationScheme;
        return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(authenticationScheme, displayName,
            options);
    }

    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder,
        Action<CustomAuthenticationOptions>? options)
    {
        var scheme = CustomAuthenticationDefaults.AuthenticationScheme;
        var displayName = CustomAuthenticationDefaults.AuthenticationScheme;
        return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(scheme, displayName,
            options);
    }

    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder)
    {
        var scheme = CustomAuthenticationDefaults.AuthenticationScheme;
        var displayName = CustomAuthenticationDefaults.AuthenticationScheme;
        Action<CustomAuthenticationOptions>? configureOptions = null;
        return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(scheme, displayName,
            configureOptions);
    }
}

// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions?view=aspnetcore-9.0
public sealed class CustomAuthenticationOptions : AuthenticationSchemeOptions;

public sealed class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
{
    public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationOptions> options,
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