#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    // Create a Policy
    opts.AddPolicy(AuthorizationConsts.AllowEditingPolicy, policy =>
    {
        // Add a Requirement
        policy.Requirements.Add(new SameUserRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");
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

public sealed class SameUserRequirement : IAuthorizationRequirement;

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

public static class AuthorizationConsts
{
    public const string AllowEditingPolicy = "AllowEditingPolicy";
}