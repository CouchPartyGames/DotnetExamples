#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Very Simple Minimal API
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World");
app.MapGet("/local", LocalFunction);
app.MapGet("/delegate", EndpointExtensions.AnotherHelloWorld);

app.Run();

string LocalFunction() => "This is local function";

public static class EndpointExtensions
{
    public static string AnotherHelloWorld() => "Hello World";
}