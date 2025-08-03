#!/usr/bin/env dotnet
#:package TUnit@0.25.21

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;


public class TestAuthorizationServicePolicy 
{
    private readonly IAuthorizationService  _authService;

    public TestAuthorizationServicePolicy()
    {
        _authService = BuildAuthorizationService(services =>
        {
            services.AddAuthorizationCore(opts =>
            {
                opts.AddPolicy(name, policy);
                opts.AddPolicy("Over18", new AuthorizationPolicyBuilder().AddRequirements(someRequirement).Build());
            });
        });
    }
    
    // Helper Method
    private IAuthorizationService BuilderAuthorizationService()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        
    }


    [Test]
    public async Task AuthorizeAsync_ReturnsSucceed_WhenUserIsCreated()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        
        // Act
        var result = await _authService.AuthorizeAsync(principal, employeeReview, policyName);

        // Assert
        Assert.True(result.Succeed);
    }
}