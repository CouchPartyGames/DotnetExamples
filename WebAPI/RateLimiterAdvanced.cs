#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    opts.AddPolicy("UserRateLimit", httpContext =>
    {
        // Create User Policy
        string? userId = httpContext.User.FindFirstValue("userId");
        if (!string.IsNullOrWhiteSpace(userId))
        {
            return RateLimiterPartition.GetTokenBucketLimiter(userId,
                _ = new TokenBucketRateLimiterOptions {});
        }
        
        // Create Anonymous
        return RateLimiterPartition.GetFixedWindowLimiter("anonymous", _ => new FixedWindowRateLimiter
        {
            PermitLimit = 5;    // how many requests you can have per window
            Window = TimeSpan.FromMinutes(1);
        });
    });

    opts.AddPolicy("IpAddressRateLimit", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
        //return RateLimiterPartition.Create
    });
    
});
var app = builder.Build();
app.MapGet("/", () => "Hello World");

app.MapGet("/by-ip", () =>
{

}).RequireRateLimiting("IpAddressRateLimit");

app.MapGet("/by-user", () =>
{
    
}).RequireRateLimiting("UserRateLimit");

app.Run();