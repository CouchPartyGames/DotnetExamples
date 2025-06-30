#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Asp.Versioning.Http@8.1.0

// Purpose: Example of using API versioning with your Minimal API
//
// https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using Asp.Versioning.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiVersioning(options => {
        // Default Version
    options.DefaultApiVersion = new ApiVersion(1);
        // Add a Header that sets a header in the response
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
});

var app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();


app.MapGet("v{version:apiVersion}/hello", () => "Hello World")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.MapGet("v{version:apiVersion}/hello", () => "Hello World 2")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(2);

app.MapGet("v{version:apiVersion}/deprecated", () => "This endpoint is deprecated!")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1)
    .HasDeprecatedApiVersion(new ApiVersion(1)); 

// Route Group Example
RouteGroupBuilder group = app.MapGroup("api/v{version:apiVersion}");
group.MapGet("/test1", () => "hello 1 from route group");
group.MapGet("/test2", () => "hello 2 from route group");   

app.Run();