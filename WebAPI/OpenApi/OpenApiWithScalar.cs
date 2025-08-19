#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Scalar.AspNetCore@2.4.22

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

app.MapGet("/", () => Results.Redirect("/scalar"))
    .Produces(302);
app.MapPost("/v1", () => "Hello World");
app.MapPut("/v1", () => "Hello World");
app.MapDelete("/v1", () => "Hello World");
app.MapPatch("/v1", () => "Hello World");
app.Run();