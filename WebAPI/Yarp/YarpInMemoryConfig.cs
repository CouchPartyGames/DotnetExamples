#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.1.0

using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Define your routes and clusters in code
var routes = new[]
{
    new RouteConfig()
    {
        RouteId = "myRoute", // Unique identifier for the route
        ClusterId = "myCluster", // Associate the route with a cluster
        Match = new RouteMatch // Define how requests are matched
        {
            Path = "/api/{**catch-all}" // Match any path starting with /api/
        }
    }
};

var clusters = new[]
{
    new ClusterConfig()
    {
        ClusterId = "myCluster", // Unique identifier for the cluster
        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "destination1", new DestinationConfig() { Address = "https://localhost:5001/" } } // Define backend destination(s)
        }
    }
};
    // Add the reverse proxy capability and load the configuration from memory
builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters);

var app = builder.Build();
    // Register the reverse proxy routes
app.MapReverseProxy();
app.Run();
