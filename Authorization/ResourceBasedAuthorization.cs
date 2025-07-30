#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

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
app.MapGet("/resource", (IAuthorizationService authorizationService) =>
{
        // Step - Verifies Rules to allow/deny access to resource
    var authorizationResult = await authorizationService.AuthorizeAsync("AllowEditing");
    if (authorizationResult.Succeeded)
    {
        // Allowed
        
    }
    else
    {
        // Denied
        return Results.Unauthorized();
    }

});
app.Run();

// Step - Create/Define a Requirement
//  Requirement just needs inherit from IAuthorizationRequirement
public sealed class SameUserRequirement : IAuthorizationRequirement;

// Step - Create a Handler for the Requirement
//  Create your logic to allow, deny or just return (no succeed or fail) for a Requirement
public sealed class DocumentAuthorizationHandler : AuthorizationHandler<SameUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SameUserRequirement requirement,
        Document resource)
    {
        if (context.User.Identity?.Name == resouce.Name)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}