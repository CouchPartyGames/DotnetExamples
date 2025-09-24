#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Necessary when you have different CORS policies for different endpoints
var builder = WebApplication.CreateBuilder(args);

// Step - Register Cors Policies
//  https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.corsservicecollectionextensions.addcors?view=aspnetcore-9.0
builder.Services.AddCors(opts =>
{
    // #1 - Add a CORS Policy
    opts.AddPolicy("LocalhostOpen", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Step - Add Cors Policy to Endpoint
app.MapGet("/", () => "Hello World")
    .RequireCors("LocalhostOpen");

app.Run();
