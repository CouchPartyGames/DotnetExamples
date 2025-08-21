#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*

using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
// Step - Register OpenAPI
//  Generate Multiple Documents
builder.Services.AddOpenApi("v1", opts =>
{
    opts.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
});

// Step - Register Output Caching
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});

builder.Services.AddOpenApi("v2");
var app = builder.Build();

// Step - OpenApi Middleware
//  Add Caching to OpenAPI responses
app.UseOutputCache();
app.MapOpenApi("/openapi/custom")
    .CacheOutput();

app.Run();