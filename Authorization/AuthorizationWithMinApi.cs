#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Minimal API
//
// Examples of Authorization using Minimal API
// Examples of using Multiple Policies on an Endpoint
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// By default, the endpoint allows anyone to access
app.MapGet("/", () => "Hello World");

// Explicitly tell the endpoint to allow anyone
app.MapGet("/allow-anyone", () => "Allow Anyone")
    .AllowAnonymous();

// This example uses attributes to allow anonymous access to the endpoint
app.MapGet("/login", [AllowAnonymous] () => "This endpoint is for all roles.");

// Tell the endpoint that only authenticated user are allowed
app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();


// This example uses attributes to protect the endpoint, only an authenticated user is allowed 
//  to access this resource.
app.MapGet("/protected", [Authorize] () => "Protected");


app.MapGet("/protected-multi-policies", () => "Protected Multi-Policies")
    .RequireAuthorization("MustBe21")
    .RequireAuthorization("SomePolicy");

app.Run();