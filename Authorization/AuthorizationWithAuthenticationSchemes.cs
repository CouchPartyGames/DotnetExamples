#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Authorization using Authentication Schemes in Minimal API 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
        // Create a policy that requires Cookie Authentication
    opts.AddPolicy("OnlyCookies", builder =>
    {
        builder.AddAuthenticationSchemes( ["Cookie"] );
    });
});
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World");

app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();

app.MapGet("/protected", 
    [Authorize(AuthenticationSchemes = "JwtBearer,Cookie")] 
    () => "protected");

app.MapGet("/protected",
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Authorize(AuthenticationSchemes = "Cookie")]
    () => "protected 2");)

app.MapGet("/cookies-only", () => "cookies-only")
    .RquireAuthorization("OnlyCookies");

app.Run();