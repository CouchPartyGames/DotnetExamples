#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview*

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer();
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
app.Run();


// Add Claims Transformation 
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
        var identity = new ClaimsIdentity();
        //identity.AddClaim(new Claim('SomeClaim', 'SomeClaimValue'));
        principal.AddIdentity(identity);
        
        return Task.FromResult(principal);
    }
}