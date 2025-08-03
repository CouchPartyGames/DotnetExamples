#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Github OAuth Authentication 
using Microsoft.AspNetCore.Authentication;
using System;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("github", opts =>
    {
        opts.ClientId = Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID");
        opts.ClientSecret = Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET");
        
        opts.SignInScheme = "cookie";
        opts.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        opts.TokenEndpoint = "https://github.com/login/oauth/access_token";
        opts.CallbackPath = "/signin-github";
		opts.SaveTokens = true;
        
        opts.UserInformationEndpoint = "https://api.github.com/user";
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext context) =>
{
    return context.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
});
app.MapGet("/login", (HttpContext ctx) =>
{
    return Results.Challenge(
        new AuthenticationProperties() { RedirectUri = "http://localhost:5000/" },
        authenticationSchemes: [ "github" ]);
});
app.MapGet("/protected", () => "Protected").RequireAuthorization();
app.Run();
