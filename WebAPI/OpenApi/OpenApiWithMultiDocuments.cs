#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*

var builder = WebApplication.CreateBuilder(args);

// Public document — only public API endpoints.
builder.Services.AddOpenApi("public", options =>
{
    options.ShouldInclude = description =>
        description.RelativePath?.StartsWith("public", StringComparison.OrdinalIgnoreCase) ?? false;
});

// Internal document — only private/internal API endpoints.
builder.Services.AddOpenApi("internal", options =>
{
    options.ShouldInclude = description =>
        description.RelativePath?.StartsWith("private", StringComparison.OrdinalIgnoreCase) ?? false;
});

var app = builder.Build();

// Serves /openapi/public.json and /openapi/internal.json
app.MapOpenApi();

app.MapGet("/public/test", () => "Hello World");
app.MapGet("/private/test", () => "Hello World");

app.Run();