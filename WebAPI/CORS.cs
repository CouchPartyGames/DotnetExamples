#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Simple example of using CORS middleware
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Step - Register Cors Policies
//  https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.corsservicecollectionextensions.addcors?view=aspnetcore-9.0
builder.Services.AddCors(opts =>
{
    opts.AddPolicy(CorsPolicyExtensions.ProductionPolicyName, 
        CorsPolicyExtensions.ProductionPolicy());
    
    opts.AddPolicy("LocalhostOpen", corsOpts =>
    {
        corsOpts
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();

// Step - Apply Specific Cors Middleware
//  Order is important, UseCors() must be called before UseResponseCache()
app.UseCors(CorsPolicyExtensions.ProductionPolicyName);

app.MapGet("/", () => "Hello World");
app.Run();

// A Better approach to defining CORS Policy Names and Policies
// This allows for easier Testing of CORS policies 
public static class CorsPolicyExtensions
{
    public const string ProductionPolicyName = "Production";
    
    public static CorsPolicy ProductionPolicy() => 
        new CorsPolicyBuilder()
            .WithOrigins("https://my.site")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .Build();
}
