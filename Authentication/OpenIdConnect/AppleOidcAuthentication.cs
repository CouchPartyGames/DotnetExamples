#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview*

// Apple OIDC Authentication
// https://www.scottbrady.io/openid-connect/implementing-sign-in-with-apple-in-aspnet-core
// https://github.com/scottbrady91/AspNetCore-SignInWithApple-Example/tree/main/ScottBrady91.SignInWithApple.Example
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = "Apple";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenConnectId("Apple", opts =>
    {
        opts.ClientId = Environment.GetEnvironmentVariable("APPLE_CLIENT_ID");
        opts.ClientSecret = Environment.GetEnvironmentVariable("APPLE_CLIENT_SECRET");

        opts.Authority = "https://appleid.apple.com";
        opts.CallbackPath = "/signin-apple";
        opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        opts.ResponseType = "code id_token";
        opts.ResponseMode = "form_post";
        opts.UsePkce = false;
        opts.DisableTelemetry = true;

        opts.Scope.Clear();
        opts.Scope.Add("openid");
        opts.Scope.Add("email");
        opts.Scope.Add("name");
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization(); // Use authorization middleware

// Define a protected endpoint that requires authentication
app.MapGet("/protected", (HttpContext context) =>
    {
        return $"Hello, {context.User.Identity?.Name}! You are authenticated.";
    })
    .RequireAuthorization(); // Requires authenticated user

app.MapGet("/signin-apple", async (HttpContext context) =>
{
    var info = await context.AuthenticateAsync("Apple");
    if (info.Succeeded)
    {
        return Results.Ok("User authenticated!");
    }
    
    return Results.BadRequest("Failed to authenticate!");
});
app.Run();

public static class AppleTokenGenerator
{
    private const string Issuer = "";
    private const string Audience = "https://appleid.apple.com";
    private const string Subject = "";

    public static ECDsaSecurityKey FileToKey(string fileName)
    {
        string pem = File.ReadAllText(fileName);
        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem);
        return new ECDsaSecurityKey(ecdsa);
    }
    
    public static ECDsaSecurityKey GetKey(string p8Base64)
    {
        var ecdsa = ECDsa.Create();
        ecdsa?.ImportPkcs8PrivateKey(Convert.FromBase64String(p8Base64), out _);
        return new ECDsaSecurityKey(ecdsa);
    }
    
    public static string NewToken(ECDsaSecurityKey ecdsaKey)
    {
        var now = DateTime.UtcNow;
        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Expires = now.AddMinutes(5),
            IssuedAt = now,
            NotBefore = now,
            SigningCredentials = new SigningCredentials(ecdsaKey, SecurityAlgorithms.EcdsaSha256)
        });
    }
}
