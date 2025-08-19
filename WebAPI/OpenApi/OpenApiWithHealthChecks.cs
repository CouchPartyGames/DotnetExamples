#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Scalar.AspNetCore@2.4.22


// Simple Health Check
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Step - Register Health Check
builder.Services.AddHealthChecks();
// Step - Register OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// OpenAPI
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0
// http://localhost:5000/openapi/v1.json
app.MapOpenApi();

// Scalar
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-9.0#use-scalar-for-interactive-api-documentation
// http://localhost:5000/scalar/v1
app.MapScalarApiReference();

app.MapGet("/", () => "Hello World");

app.MapHealthChecks("/health");
/*
// Step - Add health check endpoint to OpenAPI
app.MapGet("/health", async (HealthCheckService healthCheckService) =>
{
    var result = await healthCheckService.CheckHealthAsync();
    return result.Status == HealthStatus.Healthy ? Results.Ok(result) : Results.StatusCode(503);
}).WithOpenApi(operation => new(operation)
{
    Summary = "Health Check",
    Description = "Returns the health status of the application"
});
*/

app.Run();