#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package AspNetCore.SecurityKey@2.3.0


// https://github.com/loresoft/AspNetCore.SecurityKey
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();
