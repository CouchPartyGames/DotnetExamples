#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplicationn.CreateBuilder(args);
var app = builder.Build();
app.Run();
