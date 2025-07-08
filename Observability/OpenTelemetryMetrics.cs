#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Extensions.Hosting@1.12.*
#:package OpenTelemetry.Exporter.Console@1.12.*

using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .WithMetrics(x => x.AddConsoleExporter());

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();