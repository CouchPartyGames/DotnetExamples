#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
	// Add Problem Details
builder.Services.AddProblemDetails();
var app = builder.Build();

	// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

	// Returns the Problem Details response for (empty) non-successful responses
app.UseStatusCodePages();
app.MapGet("/badrequest", () => TypedResults.BadRequest());
app.MapGet("/error", () => TypedResults.InternalServerError());
app.Run();
