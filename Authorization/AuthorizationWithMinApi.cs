#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// Simple Authorization using Minimal API
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// By default, the endpoint allows anyway to access
app.MapGet("/", () => "Hello World");

// Explicitly tell the endpoint to allow anyone
app.MapGet("/allow-anyone", () => "Allow Anyone")
    .AllowAnonymous();

// This example uses attributes to allow anonymous access to the endpoint
app.MapGet("/login", [AllowAnonymous] () => "This endpoint is for all roles.");

// Tell the endpoint that only authenticated user are allowed
app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();

app.Run();