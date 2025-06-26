#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-preview*
#:package Microsoft.Extensions.DependencyInjection@10.*-preview*

// Google Authentication
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-9.0
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = "Google";
        opts.DefaultChallengeScheme = "Google";
    })
    .AddCookie()
    .AddGoogle("Google", opts => 
    {
        opts.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        opts.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
        
        opts.CallbackPath = "/signin-google";
        opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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

app.MapGet("/signin-google", async (HttpContext httpContext) =>
{
    var info = await httpContext.AuthenticateAsync("Google");
    if (info.Succeeded)
    {
        // User authenticated successfully
        var user = info.Principal;
        // You can access claims like name, email, etc. from user.Claims


        // If you are storing user information, do so here
        // You can also redirect the user to another page
        return Results.Ok("User authenticated successfully");
    }
    else
    {
        return Results.BadRequest("Authentication failed");
    }
});

app.Run();
