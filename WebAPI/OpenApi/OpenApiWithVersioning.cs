#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Scalar.AspNetCore@2.7.*
#:package Asp.Versioning.Http@8.1.0
#:package Asp.Versioning.Mvc.ApiExplorer@8.1.0

using Scalar.AspNetCore;
using Asp.Versioning;
using Asp.Versioning.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddApiVersioning(options => {
    // Default Version
    options.DefaultApiVersion = new ApiVersion(1);
    // Add a Header that sets a header in the response
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    // Replace the placeholder with the actual version
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Step - Create Version Set
//   This creates versions 1 and 2
ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0
// http://localhost:5000/openapi/v1.json
app.MapOpenApi();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-9.0#use-scalar-for-interactive-api-documentation
// http://localhost:5000/scalar/v1
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"))
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1)
    .Produces(302);

app.MapPost("/v{version:apiVersion}/hello", () => "Hello World")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.MapPut("/v{version:apiVersion}/hello", () => "Hello World")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.MapDelete("/v{version:apiVersion}/hello", () => "Hello World")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.MapPatch("/v{version:apiVersion}/hello", () => "Hello World")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(1);

app.Run();
