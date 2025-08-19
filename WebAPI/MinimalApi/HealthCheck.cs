#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false     

// Simple Health Check
//
// To use the 2nd method, you must disable AOT
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
// Step - Register Health Check
builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapGet("/", () => Results.Content("<a href=/health>Health Check #1</a> <a href=/health-minapi>Health Check #2</a>", "text/html"));

// Step - Use Middleware to respond to /health 
app.MapHealthChecks("/health");

// Step - Or Another Method (Optional)
app.MapGet("/health-minapi", async (HealthCheckService healthCheckService) =>
{
    var result = await healthCheckService.CheckHealthAsync();
    return result.Status == HealthStatus.Healthy ? Results.Ok(result) : Results.StatusCode(503);
});
    
app.Run();