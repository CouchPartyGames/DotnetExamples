#!/usr/bin/env dotnet
#:package TUnit@0.25.21

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public class TestAuthorizationHandler
{
    private EmployeeReview _employeeReview = new EmployeeReview();
    
    [Test]
    public async Task HandleAsync_ReturnsSucceed_WhereUserIsCreator()
    {
        // Arrange
        var principal = new ClaimsPrincipal([ new ClaimsIdentity(
            [ new Claim(),
            new Claim()]
            ) ]);

        var authContext = new AuthorizationHandlerContext(
            new List<IAuthorizationRequirment>
            {
                new Requirement()     
            }, principal, _employeeReview);
        
        // Act
        var sut = new MyAuthorizationHandler<Requirement>();
        await sut.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);
    }
}
