#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package KubernetesClient@17.0.4
#:package AgonesSDK@1.50.0

using Agones;

// Very Simple Minimal API
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.MapPost("/", () =>
{
    // Load from in-cluster configuration:
    var config = KubernetesClientConfiguration.InClusterConfig()

    // Use the config object to create a client.
    var client = new Kubernetes(config);

    var agones = new AgonesSDK();

});
app.Run();
