#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

/*
 *	RFC-7807
    type → A URI that identifies the problem type. (Could point to documentation, or to the RFC section itself.)
    title → A short, human-readable summary of the problem.
    status → The HTTP status code (400, 404, 500, etc.).
    detail → A human-readable explanation of what went wrong.
    instance → The specific request path or resource that caused the error.
 */

var builder = WebApplication.CreateBuilder(args);

	// Step - Add Problem Details
	//	Registers IProblemDetailsService
builder.Services.AddProblemDetails();
var app = builder.Build();

	// Step - Returns the Problem Details response for (empty) non-successful responses
	//	Normally, 404 would return status code without a body
app.UseStatusCodePages();

app.MapGet("/", () => Results.Content("<a href=/badrequest>Bad Request</a> <a href=/error>Internal Error</a> <a href=/custom-problem>Custom Problem</a>", "text/html"));
app.MapGet("/badrequest", () => TypedResults.BadRequest());
app.MapGet("/error", () => TypedResults.InternalServerError());
app.MapGet("/custom-problem", () =>
{
	return Results.Problem(
		title: "this is title",
		detail: "this is detail",
		statusCode: StatusCodes.Status404NotFound,
		instance: "/custom-problem");
});
app.Run();
