#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Authorization using Authentication Schemes in Minimal API 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World");
app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();

app.MapGet("/protected", [Authorize(AuthenticationSchemes = "JwtBearer,Cookie")] () => "protected");
app.Run();