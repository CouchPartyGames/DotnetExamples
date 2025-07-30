#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Custom Authorization with Requirement Data
//
// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iard?view=aspnetcore-9.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Step - Inject your Authorization Handler
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeAuthorizationHandler>();

var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!");

// Step - Add your Authorization Attribute to endpoint
app.MapGet("/protected", [MinimumAge(21)] () => "Protected Resource");
app.Run();


// Step - Create your Authorization Attribute 
public sealed class MinimumAgeAttribute(int age) : AuthorizeAttribute,
    IAuthorizationRequirement, IAuthorizationRequirementData
{
    public int Age { get; set; } = age;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}

// Step - Create Handler for your Authorization Attribute
//  Logic to allow/deny access to a resource based on attribute's data is placed here
public sealed class MinimumAgeAuthorizationHandler : AuthorizationHandler<MinimumAgeAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumAgeAttribute requirement)
    {
        int age = 21;   // basic example for now
        if (age >= requirement.Age)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}