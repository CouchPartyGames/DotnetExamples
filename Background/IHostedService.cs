#!/usr/bin/env dotnet


var builder = Host.CreateApplicationBuilder(args);
var host = builder.Build();
host.Run();


public sealed class MyBackgroundService : IHostedService
{
    
}