#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// Authorization in Minimal API
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    // Create a Policy
    opts.AddPolicy(AuthorizationConsts.AllowEditingPolicy, policy =>
    {
        // Add 
        policy.Requirements.Add(new SameUserRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World");
app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();
app.MapGet("/resource", (IAuthorizationService authorizationService) =>
{
    var authorizationResult = await authorizationService.AuthorizeAsync(
        AuthorizationConsts.AllowEditingPolicy);
    if (authorizationResult.Succeeded)
    {
        
    }
    else
    {
        return Results.Unauthorized();
    }

});
app.Run();


public sealed class DocumentAuthorizationHandler : AuthorizationHandler<>
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

public sealed class SameUserRequirement : IAuthorizationRequirement;

public static class AuthorizationConsts
{
    public const string AllowEditingPolicy = "AllowEditingPolicy";
}