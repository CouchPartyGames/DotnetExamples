#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

// Exception Handling was introduced in .NET 8 
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
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


public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
		var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;    // return if handled successfully
    }
}
