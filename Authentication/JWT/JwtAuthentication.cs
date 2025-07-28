#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview.*

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer();

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
        ])
    }));
    
    return Results.Text("You have logged in and received a cookie");
});
app.MapGet("/protected", () => "Secret")
    .RequireAuthorization();
app.MapGet("/user", (ClaimsPrincipal principal) =>
{
    var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
    return Results.Ok(claims);
}).RequireAuthorization();

app.Run();