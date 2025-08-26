#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web


// API Key Authentication for Minimal API using Endpoint Filters
// Uses Endpoint filter to protect specific routes
//
// curl -v -H "X-Api-Key: " http://localhost:5000/
// curl -v -H "X-Api-Key: " http://localhost:5000/protected
// curl -v -H "X-Api-Key: " http://localhost:5000/another-protected
using System.Text;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World");

    // Uses Strongly Typed (Recommended)
app.MapGet("/protected", () => "Secret")
    .AddEndpointFilter<ApiKeyEndpointFilterAuthentication>();

    // Use Delegate approach (Recommend using Strongly typed EndpointFilters - Above example)
app.MapGet("/another-protected", () => "Secret")
    .AddEndpointFilter(async (context, next) => {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyEndpointFilterAuthentication.ApiKeyHeaderName, 
                out var userApiKey))
        {
            return Results.Unauthorized();
        }
        string secretKey = "replace-me";
        if (!ApiKeyEndpointFilterAuthentication.IsMatchingAndPreventTimingAttack(userApiKey, secretKey))
        {
            return Results.Unauthorized();
        }
        
        return await(next(context)); 
    });
app.Run();


public sealed class ApiKeyEndpointFilterAuthentication : IEndpointFilter
{
    public const string ApiKeyHeaderName = "X-Api-Key";
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Get X-API-Key value from http headers
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var userApiKey))
        {
            return Results.Unauthorized();
        }
        
        // Pull this from a Secrets manager or Environment variable
        // For multi tenant, you can use the database
        string secretKey = "replace-me";
        
        // Compare API Keys to ensure they match
        if (!IsMatchingAndPreventTimingAttack(userApiKey!, secretKey))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
    
    // prevent a timing side-channel
    //
    // The goal is, the comparison should take the same amount of time regardless of the contents of the bytes, assuming they are the same length.
    // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals?view=net-10.0
    // https://vcsjones.dev/fixed-time-equals-dotnet-core/
    public static bool IsMatchingAndPreventTimingAttack(string first, string second)
    {
        ReadOnlySpan<byte> firstSpan = Encoding.UTF8.GetBytes(first);
        ReadOnlySpan<byte> secondSpan = Encoding.UTF8.GetBytes(second);
		
        return CryptographicOperations.FixedTimeEquals(firstSpan, secondSpan);
    }
}
