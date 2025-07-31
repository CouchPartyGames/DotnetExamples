#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Fallback Policy
// Applies a Policy to all endpoints that DO NOT have authorization explicitly defined.
// Highly recommended to define a Fallback Policy in each project using Authorization.
//
// For example,
// Any endpoint missing [Authorization], [AllowAnonymous], RequireAuthorization, AllowAnonymous will
//  use the fallback policy.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddCookie();
builder.Services.AddAuthorization(opts =>
{
    opts.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () =>
    {
        return Results.Content("<a href=/user>User</a> <a href=/signin>Signin</a> <a href=/fallback>Fallback</a>", "text/html");
    })
    .AllowAnonymous();

/*
app.MapGet("/user", (HttpContext ctx) => 
    {
        return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList());
    })
    .RequireAuthorization();
    */

app.MapGet("/protected", [Authorize](HttpContext ctx) => 
    ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList());

app.MapGet("/fallback", () => "Without a fallback policy, this endpoint would be visible.");

app.MapGet("/signin", async (HttpContext ctx) =>
{
    var principal =  new ClaimsPrincipal(  [
        new ClaimsIdentity( [
            new Claim("Subject", "SomeUser"),
            new Claim("Role", "Admin"),
            new Claim("Email", "test@test.com")
        ] )
    ] );
    
    await ctx.SignInAsync(principal, 
        CookieAuthenticationDefaults.AuthenticationScheme);
    
    return Results.Text("You have logged in and received a cookie");
}).AllowAnonymous();

app.Run();