#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// Simple Cookie based authentication
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.SignInAsync(new ClaimsPrincipal(new []
    {
        new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], CookieAuthenticationDefaults.AuthenticationScheme)
    }));
    
    return Results.Text("You have logged in and received a cookie");
});
app.MapGet("/protected", () => "Secret")
    .RequireAuthorization();
app.Run("http://localhost:5001");