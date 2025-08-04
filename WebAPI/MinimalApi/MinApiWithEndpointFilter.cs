#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Endpoint Filters that act as middleware for individual endpoints
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/single", () => "Hello World!")
    .AddEndpointFilter<TestEndpointFilter>();

app.MapGet("/multiple", () => "Hello World!")
    .AddEndpointFilter<TestEndpointFilter>()
    .AddEndpointFilter<TestEndpointFilter2>()
    .AddEndpointFilter(async (invocation, next) =>
    {
        app.Logger.LogInformation("Before filter");
        var result = await next(invocation);
        app.Logger.LogInformation("After filter");
        return result;
    });

app.Run();


public sealed class TestEndpointFilter(ILogger<TestEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogInformation($"Before {nameof(TestEndpointFilter)} filter");
        var result = await next(context);
        logger.LogInformation($"After {nameof(TestEndpointFilter)} filter");
        return result;
    }
}
public sealed class TestEndpointFilter2(ILogger<TestEndpointFilter2> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogInformation($"Before {nameof(TestEndpointFilter2)} filter");
        var result = await next(context);
        logger.LogInformation($"After {nameof(TestEndpointFilter2)} filter");
        return result;
    }
}
