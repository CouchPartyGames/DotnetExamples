# Authorization Examples

This directory contains examples demonstrating various authorization patterns in ASP.NET Core using Minimal APIs and Controllers.

## Overview

ASP.NET Core authorization provides a way to control access to resources based on user identity, roles, claims, and custom policies. All examples use the authorization middleware pipeline and demonstrate different authorization strategies.

## Prerequisites

All examples require authentication and authorization middleware to be configured in this order:

```csharp
app.UseAuthentication();  // Must come first
app.UseAuthorization();
```

## Basic Authorization

### AuthorizationHandler.cs
Basic setup showing authorization handler structure and generic handlers.
- Demonstrates `IAuthorizationRequirement` interface
- Shows generic authorization handlers (`AuthorizationHandler<TRequirement>`)
- Generic handlers for different resource types

### AuthorizationWithMinApi.cs
Introduction to authorization in Minimal APIs.
- Anonymous access with `AllowAnonymous()` and `[AllowAnonymous]`
- Protected endpoints with `RequireAuthorization()` and `[Authorize]`
- Multiple policies on a single endpoint
- Default behavior (endpoints allow anyone by default)

### AuthorizationWithControllers.cs
Authorization using traditional WebAPI Controllers.
- Controller-level authorization
- Action-level authorization with `[Authorize]` and `[AllowAnonymous]` attributes
- Demonstrates authorization in MVC-style applications

## Policy-Based Authorization

### PolicyBasedAuthorization.cs
Comprehensive policy definition and application.
- Defining named policies with `AddPolicy()`
- Applying single and multiple policies to endpoints
- Policy requirements (authentication, claims, authentication schemes)
- Best practices: avoiding magic strings using constants

### AuthorizationWithDefaultPolicy.cs
Configure default policy for all authorized endpoints.
- Sets policy used when `[Authorize]` or `RequireAuthorization()` is called without a policy name
- Applies to endpoints with authorization but no explicit policy
- Useful for setting baseline authentication requirements

### AuthorizationWithFallbackPolicy.cs
**Highly recommended** - Configure policy for all endpoints without explicit authorization.
- Applies to endpoints missing authorization attributes/calls
- Best practice: use fallback policy to secure all endpoints by default
- Opt-out model: explicitly use `AllowAnonymous()` to make endpoints public
- Includes example with cookie authentication and sign-in flow

## Role and Claims Authorization

### RoleBasedAuthorization.cs
Authorization based on user roles.
- Policy with role requirements using `RequireRole()`
- Multiple allowed roles per policy
- Demonstrates "Admin" and "BackupUser" roles

### ClaimsBasedAuthorization.cs
Authorization based on user claims.
- Creating policies with claim requirements using `RequireClaim()`
- Demonstrates "SkillType" claim with "Normal" and "Advanced" values
- Multiple claim-based policies

## Advanced Authorization

### ResourceBasedAuthorization.cs
Authorization based on specific resource properties.
- Custom requirements (`SameUserRequirement`)
- Custom authorization handlers for resources (`AuthorizationHandler<TRequirement, TResource>`)
- Manual authorization using `IAuthorizationService`
- Resource ownership verification
- Demonstrates checking if user can edit specific document

### AuthorizationWithAuthenticationSchemes.cs
Integration with authentication schemes.
- Policies tied to specific authentication schemes
- Useful when using multiple authentication methods (cookies, JWT, etc.)

### AuthorizationQuitOnFail.cs
Control handler evaluation on failure.
- Demonstrates stopping evaluation on first failure
- Handler chain behavior

### AuthorizationWithCustomAttributes.cs
Custom authorization attributes.
- Creating reusable custom authorization attributes
- Extending built-in authorization attributes

### AuthorizationWithCustomAttributeData.cs
Working with authorization metadata and attribute data.
- Accessing authorization metadata
- Custom authorization attribute processing
- Advanced attribute-based scenarios

### AuthorizationWithExtensions.cs
Extension methods for cleaner authorization code.
- Custom extension methods for policy registration (`AddAuthorizationPolicies()`)
- Custom extension methods for endpoints (`RequirePlayerRole()`, etc.)
- Demonstrates organized, reusable authorization code
- Multiple role-based policies with assertions

## Testing

### TestAuthorizationHandlers.cs
Unit testing authorization handlers.
- Testing resource-based authorization handlers
- Setting up test users with claims
- Creating authorization contexts for testing
- Uses TUnit testing framework
- Demonstrates testing `DocumentAuthorizationHandler`

### TestAuthorizationPolicies.cs
Unit testing authorization policies.
- Testing policy requirements
- Verifying policy behavior

## Running the Examples

Each file is a self-contained executable C# script that can be run using:

```bash
dotnet run --file <filename.cs>
```

Or with the shebang:

```bash
chmod +x <filename.cs>
./<filename.cs>
```

## Key Concepts

### Authorization vs Authentication
- **Authentication**: Verifies who the user is (identity)
- **Authorization**: Determines what the user can access (permissions)

### Policy Components
- **Policy**: Named set of requirements
- **Requirement**: Individual authorization rule (`IAuthorizationRequirement`)
- **Handler**: Logic to evaluate requirement (`AuthorizationHandler<T>`)

### Middleware Order
Always register authentication before authorization:
```csharp
app.UseAuthentication();  // First
app.UseAuthorization();   // Second
```

## Best Practices

1. **Use Fallback Policy**: Set a fallback policy requiring authentication by default
2. **Avoid Magic Strings**: Define policy names as constants
3. **Separate Concerns**: Keep authorization logic in handlers, not endpoints
4. **Use Extension Methods**: Create extensions for cleaner endpoint configuration
5. **Resource-Based Authorization**: Use `IAuthorizationService` for resource-specific checks
6. **Test Your Handlers**: Write unit tests for custom authorization handlers

## Additional Resources

index.html - HTML file for testing endpoints (if needed)
