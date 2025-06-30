#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// Very Simple Minimal API
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.Run();