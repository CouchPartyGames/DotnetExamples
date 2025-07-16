#!/usr/bin/env dotnet
#:sdk Microsoft.Sdk.NET.Web

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.MapGet("/", () => "Hello");
app.Run();
