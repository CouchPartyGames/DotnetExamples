#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-9.0
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<FactoryMiddleware>();
var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine("Before 1st Invoke");
    await next.Invoke();
    Console.WriteLine("After 1st Invoke");
});
app.UseMiddleware<ConventionMiddleware>();
app.UseMiddleware<FactoryMiddleware>();
app.Use(async (context, next) =>
{
    Console.WriteLine("Before 2nd Invoke");
    await next.Invoke();
    Console.WriteLine("After 2nd Invoke");
});

app.Run();


public sealed class ConventionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("Before Convention Invoke");
        await next(context);
        Console.WriteLine("After Convention Invoke");
    }
}

// Preferred method since it's strongly typed
public sealed class FactoryMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine("Before Factory Invoke");
        await next(context);
        Console.WriteLine("After Factory Invoke");
    }
}