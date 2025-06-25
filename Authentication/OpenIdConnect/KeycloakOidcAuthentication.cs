#!/usr/bin/env -S dotnet run
#:sdk Microsoft.Sdk.NET.Web

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.MapGet("/", () => "Hello");
app.Run();
