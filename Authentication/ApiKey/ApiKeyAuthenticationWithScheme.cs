#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddApiKey();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

public static class ApiKeyAuthenticationDefaults
{
    public const string AuthenticationScheme = "ApiKey";
}

public static class ApiKeyAuthenticationExtensions
{
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder) => 
        builder.AddHmac(HmacAuthenticationDefaults.AuthenticationScheme, null, _  => {});
    
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder,
        Action<HmacAuthenticationOptions> options) =>
        builder.AddHmac(HmacAuthenticationDefaults.AuthenticationScheme, null, options);
    
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string? authenticationScheme, 
        Action<HmacAuthenticationOptions>? options) => 
        builder.AddHmac(HmacAuthenticationDefaults.AuthenticationScheme, null, options);

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, 
        string? displayName, 
        Action<HmacAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(authenticationScheme, displayName,
            configureOptions);
    }
}

// Step - Allow for Customization of the Handler
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions?view=aspnetcore-9.0
public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Name { get; set; }
}

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
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
