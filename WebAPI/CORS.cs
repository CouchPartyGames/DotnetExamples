#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Simple example of using CORS middleware
// SetPreflightMaxAge as a simple tweak that can really reduce preflight overhead and boost performance
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Step - Register Cors Policies
//  https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.corsservicecollectionextensions.addcors?view=aspnetcore-9.0
builder.Services.AddCors(opts =>
{
    // #1 - Add a CORS Policy
    opts.AddPolicy("LocalhostOpen", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    
    // #2 - Another way to add a CORS policy
    opts.AddPolicy(CorsPolicyExtensions.ProductionPolicyName, 
        CorsPolicyExtensions.ProductionPolicy());
});
var app = builder.Build();

// Step - Apply Specific Cors Middleware
//  Order is important, UseCors() must be called before UseResponseCache()
app.UseCors(CorsPolicyExtensions.ProductionPolicyName);

app.MapGet("/", () => "Hello World");
app.Run();

// A Better approach to defining CORS Policy Names and Policies
// This allows for easier Testing of CORS policies 
public static class CorsPolicyExtensions
{
    public const string ProductionPolicyName = "Production";
    public const string DevelopmentPolicyName = "Development";
    
    public static CorsPolicy ProductionPolicy() => 
        new CorsPolicyBuilder()
            .WithOrigins("https://my.site")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
                // Cache preflight (Max-Age) to reduce OPTIONS chatter
                // Browsers cap duration (e.g., Chrome ~2h)
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10))
            .Build();

    // Allow Everything
    // This is should only be used for development and testing environments
    public static CorsPolicy DevelopmentPolicy() =>
        new CorsPolicyBuilder()
            .WithAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true) // allow any site
            .Build();

    public static CorsPolicy LocalhostPolicy() =>
        new CorsPolicyBuilder()
            .WithAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin =>
            {
                return origin.EndsWith("localhost");
            })
            .Build();
}