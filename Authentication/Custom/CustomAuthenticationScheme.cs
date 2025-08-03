#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Example of a Custom Authentication Scheme
//
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddCustom("MyCustomScheme", opts =>
    {
        opts.Name = "hello";
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!");
app.Run();


// Step - Defaults 
public static class CustomAuthenticationDefaults
{
    public const string AuthenticationScheme = "Custom";
}

// Step - Extension of Authentication Injection
public static class CustomAuthenticationExtensions
{

    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder) => 
        builder.AddCustom(CustomAuthenticationDefaults.AuthenticationScheme, null, _  => {});
    
    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder,
        Action<CustomAuthenticationOptions> options) =>
            builder.AddCustom(CustomAuthenticationDefaults.AuthenticationScheme, null, options);
    
    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder, string? authenticationScheme, 
        Action<CustomAuthenticationOptions>? options) => 
            builder.AddCustom(CustomAuthenticationDefaults.AuthenticationScheme, null, options);

    public static AuthenticationBuilder AddCustom(this AuthenticationBuilder builder, string authenticationScheme, 
        string? displayName, 
        Action<CustomAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(authenticationScheme, displayName,
            configureOptions);
    }
}

// Step - Allow for Customization of the Handler
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions?view=aspnetcore-9.0
public sealed class CustomAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Name { get; set; }
}

// Step - Create a Handler
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