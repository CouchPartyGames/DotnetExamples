#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// This should only be used in the Development environment
app.UseDeveloperExceptionPage();
app.Run();
