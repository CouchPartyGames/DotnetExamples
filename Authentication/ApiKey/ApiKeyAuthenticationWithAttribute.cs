#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// API Key Authentication for MVC
// Uses Service Filter Attribute to protect specific endpoints
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
var app = builder.Build();
app.UseRouting();
app.Run();


public sealed class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter)) {}
    
}

public sealed class ApiKeyAuthorizationFilter : IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];
        if (String.IsNullOrWhiteSpace(apiKey))
        {
            context.Result = new UnauthorizedResult();
        }
        
        // Pull this from a Secrets manager or Environment variable
        // For multi tenant, you can use the database
        string secretApiKey = "SECRET-API-KEY";

        if (!IsMatchingAndPreventTimingAttack(apiKey, secretApiKey))
        {
            context.Result = new UnauthorizedResult();
        }
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


[ApiKey]
[Route("[controller]")]
[ApiController]
public sealed class HomeController : ControllerBase
{
    
}
