#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-9.0
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine("Before 1st Invoke");
    await next.Invoke();
    Console.WriteLine("After 1st Invoke");
});
app.Use(async (context, next) =>
{
    Console.WriteLine("Before 2nd Invoke");
    await next.Invoke();
    Console.WriteLine("After 2nd Invoke");
});

app.Run();