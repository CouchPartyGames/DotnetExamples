#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-preview*
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Scalar.AspNetCore@2.4.22
#:property PublishAot=false

// Example of Using External Providers like Google, Apple, etc
//
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();

// Step - Setup SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite("Data Source=identity.sql"));

// Step - Setup Identity and Entity Store
//  AddIdentityCore is used b/c we don't care about roles in this example
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<SignInManager<IdentityUser>>();

// Step - Add Authentication Schemes
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = "Google";
    })
    .AddCookie()
    .AddGoogle(opts =>
    {
        opts.ClientId = builder.Configuration["Google:ClientId"];
        opts.ClientSecret = builder.Configuration["Google:ClientSecret"];

        opts.Scope.Clear();
        opts.Scope.Add("openid");
        opts.Scope.Add("profile");
        opts.Scope.Add("email");
        
        opts.ClaimActions.MapJsonKey("picture", "picture");
        opts.SaveTokens = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/auth/google", EndpointExtensions.GoogleLogin);
app.MapGet("/auth/google/callback", EndpointExtensions.GoogleCallback);
app.MapGet("/me", EndpointExtensions.UserDetails)
    .RequireAuthorization();
app.MapGet("/logout", () => "Logout")
    .RequireAuthorization();

app.Run();
    
    
public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}

public static class IdentityExtensions
{
    public const string LoginAction = "login";
    public const string RegisterAction = "register";
    
    public static AuthSettings NewAuthSettings(string provider, string redirectUrl = "http://localhost:3000") =>
        new AuthSettings(
            new List<string> { provider },
            provider,
            $"http://localhost:5000/auth/{provider.ToLower()}/callback/?ReturnUrl={redirectUrl}");

    public static string GetClaimsTable(ClaimsPrincipal user)
    {
        var html = "<h2>Claims</h2><table><tr><th>Key</th><th>Value</th></tr>";
        foreach (var claim in user?.Claims) {
            html += $"<tr><td>{claim.Type}</td><td>{claim.Value}</td></tr>";
        }
        html += "</table>";
        return html;
    }
}

public static class EndpointExtensions
{
    public static async Task GoogleLogin([FromServices] SignInManager<IdentityUser> signInManager, HttpContext context)
    {
        var properties = IdentityExtensions.NewAuthSettings("Google");
    
        var authenticationProperties = signInManager.ConfigureExternalAuthenticationProperties(
            properties.Provider,
            properties.RedirectUrl);
    
        await context.ChallengeAsync(properties.Schemes[0], authenticationProperties);
    }

    public static IResult GoogleCallback()
    {
        var redirectTo = "/me";
        return Results.Redirect(redirectTo);
    }

    public static IResult UserDetails(HttpContext context)
    {
        if (context.User is ClaimsPrincipal principal)
        {
            var pictureUrl = principal.FindFirst("picture")?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;
            
            var html = $"<h2>User: {name}</h2>";
            if (!string.IsNullOrEmpty(pictureUrl))
            {
                html += $"<img src='{pictureUrl}' alt='Profile Picture' style='width:100px;height:100px;border-radius:50%;'><br>";
            }
            html += IdentityExtensions.GetClaimsTable(principal);
            
            return Results.Content(html, "text/html");
        }

        return Results.Ok("Unknown");
    }
}

public record AuthSettings(List<string> Schemes, string Provider, string RedirectUrl);