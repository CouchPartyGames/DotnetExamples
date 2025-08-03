#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Endpoint Filters that act as middleware for individual endpoints
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("/single", () => "Hello World!")
    .AddEndpointFilter<TestEndpointFilter>();

app.MapGet("/single", () => "Hello World!")
    .AddEndpointFilter<TestEndpointFilter>()
    .AddEndpointFilter<TestEndpointFilter>();

app.Run();


public sealed class TestEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterContext next)
    {
        return await next(context);
    }
}