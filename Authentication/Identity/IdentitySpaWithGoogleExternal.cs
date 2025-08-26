#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-preview*
#:property PublishAot=false     

// This example doesn't add Identity Endpoints
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();

// Step - Setup SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite("Data Source=identity.sql"));

// Step - Add Identity Endpoints
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () =>
{
    return Results.Content("<a href=/register/google>Register</a> <a href=/login/google>Login</a> <a href=/logout>Logout</a> <a href=/me>Me</a>", "text/html");
});

app.MapGet("/me", (HttpContext context) =>
{
    var html = "<h2>Claims</h2><table><tr><th>Key</th><th>Value</th></tr>";
    foreach (var claim in context.User.Claims)
    {
        html += $"<tr><td>{claim.Type}</td><td>{claim.Value}</td></tr>";
    }
    html += "</table>";
    return Results.Content(html, "text/html");
}).RequireAuthorization();

// Register
app.MapGet("/register/google", (SignInManager<IdentityUser> signInManager,
    HttpContext context) =>
{
    var properties = LoginUtilities.NewSettings("Google", LoginUtilities.RegisterAction);
    
    var authenticationProperties = signInManager.ConfigureExternalAuthenticationProperties(
        properties.Provider,
        properties.RedirectUrl);
    
    return Results.Challenge(authenticationProperties, properties.Schemes);
});


app.MapGet("/register/google/callback", async Task<IResult> (HttpContext context) =>
{
    var signInManager = context.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
    var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
    
    var providerKey = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    var email = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    
        // User and Linked Account Information
    var externalInfo = new UserLoginInfo("Google", providerKey!, "Google");
    var user = new IdentityUser { UserName = email, 
        Email = email, 
        EmailConfirmed = true };

        // Create User
    var result = await userManager.CreateAsync(user);
    if (!result.Succeeded)
    {
        return TypedResults.InternalServerError();
    }
        // Link External to User
    result = await userManager.AddLoginAsync(user, externalInfo);
    if (!result.Succeeded)
    {
        return TypedResults.InternalServerError();
    }
    
        // Signin
    await signInManager.SignInAsync(user, isPersistent: false);
    
    return TypedResults.Ok("Success");
}).RequireAuthorization();


// Will Redirect to Google Authentication
app.MapGet("/login/google", (SignInManager<IdentityUser> signInManager, 
    HttpContext context) =>
{
    var properties = LoginUtilities.NewSettings("Google", LoginUtilities.LoginAction);
    
    var authenticationProperties = signInManager.ConfigureExternalAuthenticationProperties(
        properties.Provider,
        properties.RedirectUrl);
    
    return Results.Challenge(authenticationProperties, properties.Schemes);
});

// Will Handle Google Callback
app.MapGet("/login/google/callback", async (SignInManager<IdentityUser> signInManager, 
    HttpContext context) =>
{
    var redirectTo = "/me";
    return Results.Redirect(redirectTo);
}).WithName("GoogleLoginCallback");

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
}).RequireAuthorization();

app.Run();

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}

public static class LoginUtilities
{
    public const string LoginAction = "login";
    public const string RegisterAction = "register";
    
    public static AuthSettings NewSettings(string provider, string action, string redirectUrl = "http://localhots:3000") =>
        new AuthSettings(
            new List<string> { provider },
            provider,
            $"http://localhost:5000/{action}/{provider.ToLower()}/callback/?ReturnUrl={redirectUrl}");

    public static string GetIdentityTableHtml(ClaimsPrincipal principal)
    {
        var html = "<h2>Claims</h2><table><tr><th>Key</th><th>Value</th></tr>";
        foreach (var claim in principal.Claims)
        {
            html += $"<tr><td>{claim.Type}</td><td>{claim.Value}</td></tr>";
        }
        html += "</table>";
        return html;
    }
}

public record AuthSettings(List<string> Schemes, string Provider, string RedirectUrl);

public sealed class CreateUserService(UserManager<IdentityUser> userManager) {

	public bool NewUser(string email, string provider, string providerKey) {
        var externalInfo = new UserLoginInfo("Google", providerKey, "Google");
        var user = new IdentityUser { UserName = email, 
            Email = email, 
            EmailConfirmed = true };

            // Create User
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return TypedResults.InternalServerError();
        }
            // Link External to User
        result = await userManager.AddLoginAsync(user, externalInfo);
        if (!result.Succeeded)
        {
            return TypedResults.InternalServerError();
        }

		return true;
	}
}