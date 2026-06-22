#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*

var builder = WebApplication.CreateBuilder(args);

// Each document includes the endpoints whose WithGroupName matches the document name.

// Public document — only public API endpoints.
builder.Services.AddOpenApi("public");

// Internal document — only private/internal API endpoints.
builder.Services.AddOpenApi("internal");

var app = builder.Build();

// Serves /openapi/public.json and /openapi/internal.json
app.MapOpenApi();

app.MapGet("/public/test", () => "Hello World")
	.WithGroupName("public");

app.MapGet("/private/test", () => "Hello World")
	.WithGroupName("internal");

app.Run();
