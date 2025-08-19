#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Example of adding a Custom Health Check
var builder = WebApplication.CreateBuilder(args);

// Step - Register Sample Health Check
builder.Services.AddHealthChecks()
	.AddCheck<SampleHealthCheck>();
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.MapHealthChecks("/health");
app.Run();


// Step - Add Custom Logic
public sealed class SampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
		return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
    }
}