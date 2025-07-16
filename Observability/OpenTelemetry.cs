#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Extensions.Hosting@1.12.*
#:package OpenTelemetry.Exporter.Console@1.12.*

using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .WithLogging(x => x.AddConsoleExporter())
    .WithMetrics(x => x.AddConsoleExporter())
    .WithTracing(x => x.AddConsoleExporter());

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
