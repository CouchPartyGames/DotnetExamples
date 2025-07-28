#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

// Default Policy
// Applies a Policy to all endpoints that an authorization applied but
// DO NOT explicitly define the policy.
//
// For example,
// [Authorization], [AllowAnonymous], RequireAuthorization, AllowAnonymous 
// will use the default policy

using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
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

app.MapGet("/protected", () => "Protected")
    .RequireAuthorization();

app.MapGet("/protected2", [Authorize]() => "Protected2");

app.Run();