#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.MapHealthChecks("/health");
app.Run();