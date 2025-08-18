#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRateLimiter(opts =>
{
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opts.AddFixedWindowLimiter("fixed", cfg =>
    {
        cfg.PermitLimit = 5;    // how many requests you can have per window
        cfg.Window = TimeSpan.FromMinutes(1);   // set the timespan of that window
    });
    opts.AddPolicy("per-user", httpContext =>
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
});
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.Run();