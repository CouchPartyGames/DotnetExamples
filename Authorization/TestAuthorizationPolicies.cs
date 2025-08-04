#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package TUnit@0.25.21

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;


// Create a Policy
// 
// It's best to setup name as const and separate policy build to single for easy testing
public static class EmployeeReviewPolicies
{
    public const string ReadOnlyPolicyName = "ReadOnlyPolicy";

    public static AuthorizationPolicy GetReadOnlyPolicy() => 
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
}


// Test a Policy
public class TestAuthorizationServicePolicy 
{

    [Test]
    public async Task AuthorizeAsync_ReturnsSucceed_WhenAuthenticatedUserExists()
    {
        // Arrange
            // Create Authorization Service
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorizationCore(opts =>
        {
            opts.AddPolicy(EmployeeReviewPolicies.ReadOnlyPolicyName,
                EmployeeReviewPolicies.GetReadOnlyPolicy());
        });
        var provider = services.BuildServiceProvider();
        var authService = provider.GetRequiredService<IAuthorizationService>();
        
            // Setup User
        var claims = new List<Claim>() { new Claim(ClaimTypes.Name, "SomeGuid") };
        var identity = new ClaimsIdentity("Cookie");
        identity.AddClaims(claims);
        var user = new ClaimsPrincipal(identity);

        string? resource = null;
        var policyName = EmployeeReviewPolicies.ReadOnlyPolicyName;
        
        // Act
        var result = await authService.AuthorizeAsync(user, resource, policyName);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();
    }
}