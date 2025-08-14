#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-preview*
#:property PublishAot=false     

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;

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
        
        //opts.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Register
app.MapGet("/register/google", (SignInManager<IdentityUser> signInManager,
    LinkGenerator linkGenerator,
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

    Console.WriteLine("=== Debug External Login Callback ===");
    Console.WriteLine($"Request URL: {context.Request.GetDisplayUrl()}");
    Console.WriteLine($"Request Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
    Console.WriteLine($"User Identity: {context.User?.Identity?.Name ?? "null"}");
    Console.WriteLine($"User IsAuthenticated: {context.User?.Identity?.IsAuthenticated}");
    Console.WriteLine($"Auth Type: {context.User?.Identity?.AuthenticationType ?? "null"}");
    
    if (context.User?.Claims != null)                                                                                                                                                                                                                                                        
    {                                                                                                                                                                                                                                                                      
          foreach (var claim in context.User.Claims)                                                                                                                                                                                                                         
          {                                                                                                                                                                                                                                                                              
              Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");                                                                                                                                                                                                                          
          }                                                                                                                                                                                                                                                                                          
    }                                                                                                                                                                                                                                                                                       
    else                                                                                                                                                                                                                                                                                    
    {                                                                                                                                                                                                                                                                                  
          Console.WriteLine("No claims found");                                                                                                                                                                                                                                                
    }
    var authResult = await context.AuthenticateAsync("Google");
    Console.WriteLine($"Direct Google auth result: Succeeded={authResult.Succeeded}, Failure={authResult.Failure?.Message}");
    
    /*  THESE DOESN'T WORK CURRENTLY
    var externalInfo = await signInManager.GetExternalLoginInfoAsync();
    Console.WriteLine($"External Info: {externalInfo?.ToString() ?? "null"}");
    
    if (externalInfo is null)
    {
        Console.WriteLine("ERROR: External login info is null");
        
        // Try to get authentication result directly
        return TypedResults.InternalServerError("Failed to load external login information.");
    }

    //Console.WriteLine(externalInfo);
    */

    var providerKey = context.User.Claims.FirstOrDefault(c => c.Type == ClaimsTypes.NameIdentifier)?.Value;
    var externalInfo = new UserLoginInfo("Google", "Google", providerKey);
    var user = new IdentityUser { UserName = externalInfo.ProviderKey, Email = externalInfo.ProviderKey };

    var result = await userManager.CreateAsync(user);
    result = await userManager.AddLoginAsync(user, externalInfo);
    await signInManager.SignInAsync(user, isPersistent: false);
    return TypedResults.Ok("Success");
}).RequireAuthorization();

/*
// Will Redirect to Google Authentication
app.MapGet("/login/{provider}", (string provider, 
    SignInManager<IdentityUser> signInManager, 
    LinkGenerator linkGenerator,
    HttpContext context) =>
{
    var properties = LoginUtilities.NewSettings("Google", LoginUtilities.LoginAction);
    
    var authenticationProperties = signInManager.ConfigureExternalAuthenticationProperties(
        properties.Provider,
        properties.RedirectUrl);
    
    return Results.Challenge(authenticationProperties, properties.Schemes);
});

// Will Handle Google Callback
app.MapGet("/login/google/callback", async (HttpContext context) =>
{
    var scheme = "Google";
    var result = await context.AuthenticateAsync(scheme);
    if (!result.Succeeded)
    {
        return Results.Unauthorized();
    }

    var user = await userManager.FindByEmailAsync();
    if (!user)
    {
            // Create User
        var email = "";
        var result = await userManager.CreateAsync(GoogleUser.NewUser(email));
        if (!result)
        {
            
        }
    }

        // Add Login
    var login = GoogleUser.NewLogin();
    var loginResult = userManager.AddLoginAsync(login);
    if (!loginResult.Succeeded)
    {
        return Results.InternalServerError();
    }
    
    return Results.Redirect();

}).WithName("GoogleLoginCallback");
*/

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

    public static IdentityUser NewUser(string email) => new IdentityUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };
    
    //public static UserLoginInfo NewLogin(string string identity) => new UserLoginInfo(ProviderName, identity, ProviderName);

    //public static UserLoginInfo NewExternalLogin() => new UserLoginInfo();
    
    public static AuthSettings NewSettings(string provider, string action) =>
        new AuthSettings(
            new List<string> { provider },
            provider,
            $"http://localhost:5000/{action}/google/callback/?ReturnUrl=http://localhost:3000/");
}

public record AuthSettings(List<string> Schemes, string Provider, string RedirectUrl);