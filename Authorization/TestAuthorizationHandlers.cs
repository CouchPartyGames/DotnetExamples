#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package TUnit@0.25.21

using Microsoft.AspNetCore.Authorization;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;


public record Document(string OwnerId);

public sealed class SameUserRequirement : IAuthorizationRequirement;

public sealed class DocumentAuthorizationHandler : AuthorizationHandler<SameUserRequirement, Document>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SameUserRequirement requirement,
        Document resource)
    {
        if (context.User.Identity?.Name == resouce.OwnerId)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// Create our Main Test 
public class TestAuthorizationHandler
{
    private Document _employeeDocument = new Document("SomeGuid");
    
    [Test]
    public async Task HandleAsync_ReturnsSucceed_WhereUserIsCreator()
    {
        // Arrange
        var user = new ClaimsPrincipal([ new ClaimsIdentity(
            [ new Claim(),
            new Claim()]
            ) ]);

        var authContext = new AuthorizationHandlerContext(
            new List<IAuthorizationRequirement> { new SameUserRequirement() }, 
            user, 
            _employeeDocument);
        
        // Act
        var sut = new DocumentAuthorizationHandler<SameUserRequirement>();
        await sut.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);
    }
}