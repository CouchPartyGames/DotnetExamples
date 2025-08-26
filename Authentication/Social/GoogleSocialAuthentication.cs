#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-*
#:package Microsoft.Extensions.DependencyInjection@10.*-*

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
        opts.ClientId = builder.Configuration["Google:ClientId"];
        opts.ClientSecret = builder.Configuration["Google:ClientSecret"];
        
        opts.CallbackPath = "/signin-google";
        opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization(); // Use authorization middleware

app.MapGet("/",  () => "Google Social Authentication Example!");
// Define a protected endpoint that requires authentication
app.MapGet("/protected", (HttpContext context) =>
    {
        return $"Hello, {context.User.Identity?.Name}! You are authenticated.";
    })
    .RequireAuthorization(); // Requires authenticated user

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //await context.SignOutAsync("Google");
    
    return "Signed out";
}).RequireAuthorization();
app.Run();
