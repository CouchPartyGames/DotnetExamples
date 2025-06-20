#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthentication();
app.MapGet("/", () => "hello world!");
app.MapGet("/protected", () => "Secret");
app.Run();
