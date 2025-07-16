#!/usr/bin/env dotnet

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


// Set a Requirement
public sealed class UserCanViewReviewRequirement : IAuthorizationRequirement { }

// Authorization Handlers
public class UserIsSiteAdminHandler<TRequirement> :
    AuthorizationHandler<TRequirement, Site>
    where TRequirement : IAuthorizationRequirement
{
    protected override Task HandleRequirement(AuthorizationHandlerContext ctx, 
        TRequirement requirement, Site site)
    {
        return Task.CompletedTask;
    }
}
public class UserIsOwnerHandler<TRequirement> :
    AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
{
    protected override Task HandleRequirement(AuthorizationHandlerContext ctx, 
        TRequirement requirement)
    {
        return Task.CompletedTask;
    }
}
