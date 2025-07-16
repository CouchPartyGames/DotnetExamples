#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Simple Cookie based authentication
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
    {
        opts.Cookie.IsEssential = true;
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () =>
{
    return Results.Content("<h1>Hello World!</h1><a href=/login>Login</a>&nbsp;<a href=/protected>Protected Page</a>",
        "text/html");
});
app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.SignInAsync(new ClaimsPrincipal(new []
    {
        new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@test.com"),
        ], CookieAuthenticationDefaults.AuthenticationScheme)
    }));
    
    return Results.Text("You have logged in and received a cookie");
});
app.MapGet("/protected", (HttpContext ctx) =>
    {
        return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
    })
    .RequireAuthorization();

app.Run();