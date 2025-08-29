#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*

// Example of using OpenAPI Metadata with Minimal API
//
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-9.0&tabs=minimal-apis
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-9.0
// Note: OpenAPI works with AOT
var builder = WebApplication.CreateBuilder(args);

    // Inject OpenAPI
builder.Services.AddOpenApi();
var app = builder.Build();

    // http://localhost:5000/openapi/v1.json
app.MapOpenApi();

app.MapGet("/", () => "Hello World")
    .WithTags("Template")
    .WithSummary("Placeholder Summary")
    .WithDescription("Placeholder Description")
    .WithOpenApi();

app.MapPost("/", 
    [Tags("Muliple", "Tags")]
    [EndpointSummary("Update Template")]
    [EndpointDescription("This is a description.")]
    [EndpointName("Placeholder Name (POST)")]
    () => "Hello World");

app.MapPut("/", () => "Hello World")
    .WithTags("Template")
    .WithSummary("Update Template")
    .WithDescription("Update a Tournament Template")
    .WithOpenApi();

app.MapDelete("/", () => "Hello World")
    .WithTags("Template")
    .WithSummary("Update Template")
    .WithDescription("Update a Tournament Template")
    .WithOpenApi();

app.MapPatch("/", () => "Hello World")
    .WithTags("Template")
    .WithSummary("Update Template")
    .WithDescription("Update a Tournament Template")
    .WithOpenApi();

app.Run();
