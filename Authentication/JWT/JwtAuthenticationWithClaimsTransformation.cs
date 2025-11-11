#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.*

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer();

    // Add Claims Transformation to Dependency Injection
builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

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


// Step - Add Claims Transformation 
// Note: This process runs every single request
public sealed class CustomClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (/* if claim already exists, exit early */ false)
        {
            return Task.FromResult(principal);
        }
        
            // Add to Principal
        principal.AddIdentity(new ClaimsIdentity([ 
            new Claim("Role", "Tester"),
            new Claim("Department", "QA")
        ]));
        return Task.FromResult(principal);
    }
}