#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Fallback Policy
// Applies a Policy to all endpoints that DO NOT have authorization explicitly defined.
// Highly recommended to define a Fallback Policy in each project using Authorization.
//
// For example,
// Any endpoint missing [Authorization], [AllowAnonymous], RequireAuthorization, AllowAnonymous will
//  use the fallback policy.

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

app.MapGet("/protected2", [Authorize]() => "Protected");

app.MapGet("/fallback-applied", () => "Without a fallback policy, this endpoint would be visible.");

app.Run();