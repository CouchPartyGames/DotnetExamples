#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
// Register the global exception handler
// You can register multiple handlers
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();
var app = builder.Build();

// Use the global exception handler middleware
app.UseExceptionHandler();

app.MapGet("/", () => {
    throw new Exception("This is an unhandled exception in /");
    return "hello world";
});
app.Run();


public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Return true/false if successful
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception =  exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occurred",
                Detail = exception
            }
        });
    }
}