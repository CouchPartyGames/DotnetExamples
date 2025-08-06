#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-preview*
#:package Scalar.AspNetCore@2.4.22
#:property PublishAot=false     

// Simple Example of using .NET Core Identity for Single Page Application (SPA
//
// Add Logout Functionality since Core Identity doesn't provide it.
// Notice: Disable AOT for Scalar to properly render OpenAPI endpoints

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();

    // Step - Setup In Memory Database
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
    .AddGoogle("Google", opts =>
    {
        opts.ClientId = builder.Configuration["Google:ClientId"];
        opts.ClientSecret = builder.Configuration["Google:ClientSecret"];
        
        opts.Scope.Add("profile");
        opts.Scope.Add("email");

        opts.Events = new OAuthEvents
        {
            /*
            OnRedirectToAuthorizationEndpoint = async (context) =>
            {
                Console.WriteLine("OnRedirectToAuthorization");
                await Task.CompletedTask;
            },*/
            OnTicketReceived = async (context) =>
            {
                Console.WriteLine("OnTicketReceived");
                await Task.CompletedTask;
            }, 
            OnCreatingTicket = async (context) => {
                Console.WriteLine("OnCreatingTicket");
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();

                var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                var name = context.Principal?.FindFirst(ClaimTypes.Name)?.Value;

                if (!string.IsNullOrEmpty(email)) {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null) {
                        user = new IdentityUser {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(user);
                        if (result.Succeeded) {
                            await userManager.AddLoginAsync(user, new UserLoginInfo(context.Scheme.Name,
                                context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
                                context.Scheme.DisplayName));
                        }
                    }
                }
            } 
        };
    });

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("External", policyBuilder =>
    {
        policyBuilder
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(["Google"]);
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();

    // Step - Map Identity Endpoints/Routes
app.MapIdentityApi<IdentityUser>();

app.MapGet("/", () => Results.Redirect("/scalar"))
    .Produces(302);


app.MapGet("/user", (HttpContext context) =>
{
    return $"Hello, {context.User.Identity?.Name}";
}).RequireAuthorization("External");

/*
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Logged Out";
}).RequireAuthorization("External");
*/

    // Step - Add Logout
    // But is this really necessary/depends
app.MapGet("/mylogout", async (HttpContext context, SignInManager<IdentityUser> manager) =>
{
    var user = context.User?.Identity.Name;
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //await context.SignOutAsync(IdentityConstants.ApplicationScheme,ClaimsPrincipal.Current);
    //await manager.SignOutAsync();
    return Results.Ok($"Signed out {user}");
}).RequireAuthorization("External");


app.Run();

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}
