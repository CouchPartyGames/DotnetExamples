#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Authorization using Policies in Minimal API
//
// A very simple example of defining and applying policies to endpoints
// Best Practice: You should avoid magic strings. Instead, define policy names as constants.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    // Step - Define your Policy Name and Requirements
    opts.AddPolicy("admin-policy", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationScheme("Cookie")
            .RequireClaim("role", allowedValues: ["admin", "superuser"]);
    });
    
    // Step - Define another Policy by Name and Requirements
    opts.AddPolicy(UserAuthorizationPolicy.UserAllowPolicyName,
        policy => UserAuthorizationPolicy.GetUserAllowPolicy());
});
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");

app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();

// Step - Apply a Single Policy using the policy name
app.MapGet("/admin",  () => "Hello Admin")
    .RequireAuthorization("admin-policy");

// Step - Apply Multiple Policies using Policies Names
app.MapGet("/multiple-policies",   () => "Hello Admin")
    .RequireAuthorization("admin-policy")
    .RequireAuthorization(UserAuthorizationPolicy.UserAllowPolicyName);

app.Run();


// Step - Define Policy Name and Policy
//  This is preferred to avoid magic strings and better testability
public static class UserAuthorizationPolicy
{
    public const string UserAllowPolicyName = "UserPolicy";

    public static AuthorizationPolicyBuilder GetUserAllowPolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
}