#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Scalar.AspNetCore@2.7.*


// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-9.0&tabs=minimal-apis
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0
// http://localhost:5000/openapi/v1.json
app.MapOpenApi();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-9.0#use-scalar-for-interactive-api-documentation
// http://localhost:5000/scalar/v1
app.MapScalarApiReference();
app.MapGet("/", () => "Hello World!");

app.MapGet("/with-extension", () => "Using Extension Methods to decorate the endpoint!")
    .WithName("ExtensionMethods")
    .WithTags("extension, endpoint")
    .WithSummary("This is a summary")
    .WithDescription("This is a description");

app.MapGet("/with-attribute", 
    [EndpointName("Attributes")]
    [Tags("attribute, endpoint")]
    [EndpointSummary("This is a summary.")]
    [EndpointDescription("This is a description.")]
    () => "Using Extension Methods to decorate the endpoint!");

app.Run();