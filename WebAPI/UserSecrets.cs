#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:property UserSecretsId=dotnet-examples

using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
// By default, runfile are executed using Production release
// .NET user-secrets are intended for development only
// Therefore, you much explicitly add them
builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
var app = builder.Build();
app.MapGet("/", (IConfiguration config, IHostEnvironment hostEnvironment) =>
{
    return $"Hello World! Environment: {hostEnvironment.EnvironmentName} Secret: {config["Hello"]}";
});
app.Run();
