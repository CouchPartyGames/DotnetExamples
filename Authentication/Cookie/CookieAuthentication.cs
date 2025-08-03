#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Simple Cookie based authentication
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.Cookie.IsEssential = true;
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () =>
{
    return Results.Content("<h1>Cookie Authentication</h1><a href=/login>Login</a>&nbsp;<a href=/protected>Protected Page</a>&nbsp;<a href=/signout>Sign Out</a>",
        "text/html");
});
app.MapGet("/login", async (HttpContext ctx) =>
{
    var principal = new ClaimsPrincipal([
        new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@test.com"),
        ], CookieAuthenticationDefaults.AuthenticationScheme)
    ]);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    return Results.Text("You have logged in and received an authorization cookie");
});
app.MapGet("/signout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Text("You have signed out and deleted an authorization cookie");
}).RequireAuthorization();

app.MapGet("/protected", (HttpContext ctx) =>
    {
        return string.Join(",", ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList());
    })
    .RequireAuthorization();


app.Run();