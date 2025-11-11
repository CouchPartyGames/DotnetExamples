#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Very Simple Minimal API
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World");
app.MapGet("/local", LocalFunction);
app.MapGet("/delegate", EndpointExtensions.AnotherHelloWorld);
app.MapGet("/strongly-typed", EndpointExtensions.StronglyTyped);

// .NET 7 TypedResults
app.MapGet("/strongly-typed-anon", async Task<Results<Ok<string>, NotFound>> () =>
{
    var isError = false;
    return isError ? TypedResults.NotFound() : TypedResults.Ok("Hello World");
});

app.Run();

string LocalFunction() => "This is local function";

public static class EndpointExtensions
{
    public static string AnotherHelloWorld() => "Hello World";
    
    // .NET 7+ TypedResults
    public static Results<Ok<string>, BadRequest> StronglyTyped()
    {
        return TypedResults.Ok("Strongly Typed Example");
    }
}