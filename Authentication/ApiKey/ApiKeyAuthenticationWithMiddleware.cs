#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// API Key Authentication for Minimal Api Or MVC
// Use middleware to protect ALL Routes
//
// curl -v -H "X-Api-Key: " http://localhost:5000/
// curl -v -H "X-Api-Key: " http://localhost:5000/protected
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();
app.MapGet("/", () => "Hello World");
app.MapGet("/protected", () => "Secret");
app.Run();



public sealed class ApiKeyMiddleware(RequestDelegate next)
{
	private const string ApiKeyHeaderName = "X-Api-Key";
		
	public async Task InvokeAsync(HttpContext context)
	{
		if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var userApiKey))
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.Response.WriteAsync("API Key was not provided.");
			return;
		}
		
		// From Secrets Manager or Database
		var secretApiKey = "typically-pulled-from-the-database";
		if (!IsMatchingAndPreventTimingAttack(seretApiKey, userApiKey)) {
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.Response.WriteAsync("Unauthorized client.");
			return;
		}
		
		await next(context);
	}

	// prevent a timing side-channel
	//
	// The goal is, the comparison should take the same amount of time regardless of the contents of the bytes, assuming they are the same length.
	// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals?view=net-10.0
	// https://vcsjones.dev/fixed-time-equals-dotnet-core/
	private bool IsMatchingAndPreventTimingAttack(string first, string second)
	{
		ReadOnlySpan<byte> firstSpan = Encoding.UTF8.GetBytes(first);
		ReadOnlySpan<byte> secondSpan = Encoding.UTF8.GetBytes(second);
		
		reateCreat
	}
}
