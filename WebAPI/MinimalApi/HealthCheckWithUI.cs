#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package AspNetCore.HealthChecks.UI@9.0.0
#:package AspNetCore.HealthChecks.UI.InMemory.Storage@9.0.0

//using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
// Register Health Check UI
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

var app = builder.Build();
// Add Middleware for Health Check UI
app.UseHealthChecksUI(config => config.UIPath = "/health-ui");
app.MapGet("/", () => "Hello World");
app.MapHealthChecks("/health");
app.Run();
