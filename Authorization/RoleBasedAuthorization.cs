#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Authorization in Minimal API using Roles
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("ElevatedRights", policy =>
        policy.RequireRole("Admin", "BackupUser"));
});
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// By default, the endpoint allows anyway to access
app.MapGet("/", () => "Hello World");

// Explicitly tell the endpoint to allow anyone
app.MapGet("/allow-anyone", () => "Allow Anyone")
    .AllowAnonymous();

// Tell the endpoint that only authenticated user are allowed
app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();

app.Run();