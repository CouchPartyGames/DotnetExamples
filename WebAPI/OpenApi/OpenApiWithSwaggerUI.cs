#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Swashbuckle.AspNetCore@9.0.*

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0
// http://localhost:5000/openapi/v1.json
app.UseSwaggerUI(opts =>
{
    // Swagger UI
    // http://localhost:5000/swagger/
    opts.SwaggerEndpoint("/openapi/v1.json", "My API V1");
});
app.MapOpenApi();

app.MapGet("/", () => Redirect("/swagger/index.html"));
app.MapPost("/", () => "Hello World");
app.MapPut("/", () => "Hello World");
app.MapDelete("/", () => "Hello World");
app.MapPatch("/", () => "Hello World");
app.Run();