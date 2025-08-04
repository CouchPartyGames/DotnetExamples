#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    // Create a Policy
    opts.AddPolicy("AllowEditing", policy =>
    {
        // Add a Requirement
        policy.Requirements.Add(new SameUserRequirement());
    });
});

// Step - Inject the All Handlers for the Requirement
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();

var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");

// Step - Inject AuthorizationService
// Note: RequireAuthorization() or Authorize is NOT required for this
app.MapGet("/resource", async Task<IResult> (HttpContext context) =>
{
    var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();
    Document document = new Document("123", "Secret Document");
    
        // Step - Verifies Rules to allow/deny access to resource
    var authorizationResult = await authorizationService.AuthorizeAsync(context.User, document, "AllowEditing");
    if (authorizationResult.Succeeded)
    {
        // Allowed
        return TypedResults.Ok();
    }
    
    // Denied
    return TypedResults.Forbid();
});
app.Run();


public record Document(string OwnerId, string Name);

// Step - Create/Define a Requirement
//  Requirement just needs inherit from IAuthorizationRequirement
public sealed class SameUserRequirement : IAuthorizationRequirement;

// Step - Create a Handler for the Requirement
//  Create your logic to allow, deny or just return (no succeed or fail) for a Requirement
public sealed class DocumentAuthorizationHandler : AuthorizationHandler<SameUserRequirement, Document>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SameUserRequirement requirement,
        Document resource)
    {
        if (context.User.Identity?.Name == resource.Name)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}