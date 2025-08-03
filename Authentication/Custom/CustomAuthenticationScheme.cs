#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web


// https://github.com/loresoft/AspNetCore.SecurityKey
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();


public static class CustomAuthenticationDefaults
{
    public const string AuthenticationScheme = "Custom";
}

public static AuthenticationBuilderExtensions {
    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder) => 
        builder.AddCustom(CustomAuthenticationDefaults.AuthenticationScheme);
}

// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions?view=aspnetcore-9.0
public sealed class CustomAuthenticationOptions : AuthenticationSchemeOptions;

public sealed class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
{
    protected async Task<AuthenticateResult> HandleAuthenticateAsync()
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