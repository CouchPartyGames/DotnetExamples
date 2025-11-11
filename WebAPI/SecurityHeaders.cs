#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package NetEscapades.AspNetCore.SecurityHeaders@1.2.0


// https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaderst s
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseSecurityHeaders();
app.MapGet("/", () => "Hello World!");
app.Run();