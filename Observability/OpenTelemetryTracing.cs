#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Extensions.Hosting@1.12.*
#:package OpenTelemetry.Exporter.Console@1.12.*

using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .WithTracing(x => x.AddConsoleExporter());

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();