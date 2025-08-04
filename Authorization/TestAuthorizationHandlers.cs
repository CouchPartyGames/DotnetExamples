#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package TUnit@0.25.21

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;


// Resource Based Authorization
public record Document(string OwnerId);

public sealed class SameUserRequirement : IAuthorizationRequirement;

public sealed class DocumentAuthorizationHandler : AuthorizationHandler<SameUserRequirement, Document>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SameUserRequirement requirement,
        Document resource)
    {
        if (context.User.Identity?.Name == resource.OwnerId)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// Testing for Resource Based Authorization
public class TestAuthorizationHandler
{
    private Document _employeeDocument = new Document("SomeGuid");
    
    [Test]
    public async Task HandleAsync_ReturnsSucceed_WhereUserIsCreator()
    {
        // Arrange
        
            // Setup User
        var claims = new List<Claim>() { new Claim(ClaimTypes.Name, "SomeGuid") };
        var identity = new ClaimsIdentity("Cookie");
        identity.AddClaims(claims);
        var user = new ClaimsPrincipal(identity);

            // Setup Context
        var authContext = new AuthorizationHandlerContext(
            new List<IAuthorizationRequirement> { new SameUserRequirement() }, 
            user, 
            _employeeDocument);
        
            // Setup Handler
        var sut = new DocumentAuthorizationHandler();
        
        // Act
        await sut.HandleAsync(authContext);

        // Assert
        await Assert.That(authContext.HasSucceeded).IsTrue();
    }
}