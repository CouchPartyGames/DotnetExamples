#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);

// Register Cors Policies
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("everything-open", corsOpts =>
    {
        corsOpts
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    opts.AddPolicy("localhost-open", corsOpts =>
    {
        corsOpts
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();
// Use Cors Middleware
app.UseCors("everything-open");
app.MapGet("/", () => "Hello World");
app.Run();