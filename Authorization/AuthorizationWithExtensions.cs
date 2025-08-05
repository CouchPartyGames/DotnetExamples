#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddCookie();
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");
app.MapGet("/player", () => "Hello World")
    .RequirePlayerRole();

app.Run();


public static class AuthorizationPolicyExtensions
{
    public const string RequirePlayerRole = "RequirePlayerRole";
    public static AuthorizationPolicy RequirePlayerRolePolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("role", "Player")
            .Build();

    
    public const string RequireTournamentAdminRole = "RequireTournamentAdminRole";
    public static AuthorizationPolicy RequireTournamentAdminRolePolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("role", "Admin")
            .Build();
    
    public const string RequireTournamentOrganizerRole = "RequireTournamentOrganizerRole";
    public static AuthorizationPolicy RequireTournamentOrganizerRolePolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("role", "Organizer")
            .Build();
    
    public const string RequireAdminOrOrganizerRole = "RequireAdminOrOrganizerRole";

    public static AuthorizationPolicy RequireAdminOrOrganizerRolePolicy() =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                context.User.HasClaim("role", "Admin") ||
                context.User.HasClaim("role", "Organizer"))
            .Build();

    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(RequirePlayerRole, RequirePlayerRolePolicy())
            .AddPolicy(RequireTournamentAdminRole, RequireTournamentAdminRolePolicy())
            .AddPolicy(RequireTournamentOrganizerRole, RequireTournamentOrganizerRolePolicy())
            .AddPolicy(RequireAdminOrOrganizerRole, RequireAdminOrOrganizerRolePolicy());
    }
}

// Allow Min API Endpoints and Route Groups
//
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routegroupbuilder?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.routehandlerbuilder?view=aspnetcore-9.0
public static class AuthorizationEndpointExtensions
{
    public static IEndpointConventionBuilder RequirePlayerRole(this IEndpointConventionBuilder builder)
        => builder.RequireAuthorization(AuthorizationPolicyExtensions.RequirePlayerRole);

    public static IEndpointConventionBuilder RequireTournamentAdminRole(this RouteHandlerBuilder builder)
        => builder.RequireAuthorization(AuthorizationPolicyExtensions.RequireTournamentAdminRole);

    public static IEndpointConventionBuilder RequireTournamentOrganizerRole(this RouteHandlerBuilder builder)
        => builder.RequireAuthorization(AuthorizationPolicyExtensions.RequireTournamentOrganizerRole);

    public static IEndpointConventionBuilder RequireAdminOrOrganizerRole(this RouteHandlerBuilder builder)
        => builder.RequireAuthorization(AuthorizationPolicyExtensions.RequireAdminOrOrganizerRole);
}
