#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Swashbuckle.AspNetCore.SwaggerUi@9.0.*

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0
// http://localhost:5000/openapi/v1.json
app.MapOpenApi();
app.UseSwaggerUI(opts =>
{
    opts.SwaggerEndpoint("/openapi/v1.json", "My API V1");
});

app.MapGet("/", () => "Hello World");
app.MapPost("/", () => "Hello World");
app.MapPut("/", () => "Hello World");
app.MapDelete("/", () => "Hello World");
app.MapPatch("/", () => "Hello World");
app.Run();