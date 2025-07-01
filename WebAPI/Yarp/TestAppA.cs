#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run("http://localhost:5001");
