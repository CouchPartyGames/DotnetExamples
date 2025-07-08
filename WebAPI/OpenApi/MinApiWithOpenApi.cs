#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@9.0.*

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-9.0
var builder = WebApplication.CreateBuilder(args);

    // Inject OpenAPI
builder.Services.AddOpenApi();
var app = builder.Build();

    // http://localhost:5000/openapi/v1.json
app.MapOpenApi();

app.MapGet("/", () => "Hello World")
    .WithTags("Template")
    .WithSummary("Update Template")
    .WithDescription("Update a Tournament Template")
    .WithOpenApi();

app.MapPost("/", () => "Hello World");
    .WithTags("Template")
    .WithSummary("Update Template")
    .WithDescription("Update a Tournament Template")
    .WithOpenApi();

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