#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// Authorization in Minimal API
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
app.Run();