#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

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

		var apiKey = "typically-pulled-from-the-database";
		
		ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(apiKey);
		ReadOnlySpan<byte> userSpan = Encoding.UTF8.GetBytes(userApiKey);
		
		if (CryptographicOperations.FixedTimeEquals(span, userSpan)) {
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			await context.Response.WriteAsync("Unauthorized client.");
			return;
		}
		
		await next(context);
	}
}