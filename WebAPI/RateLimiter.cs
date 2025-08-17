#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

// Simple Rate Limiting Example using .NET Minimal API
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    // 1. Fixed Window Limiter (20 requests per 2 minutes)
    // ------------------------------------------------
    // |                2 min                        |
    // |           Blocked if limit hit             |
    // ------------------------------------------------
    // Requests reset fully after 2 minutes, blocking all requests
    // until the next window starts.
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(2);
        opt.PermitLimit = 20;
    });

    // 2. Sliding Window Limiter (20 requests per 2 minutes, 4 segments)
    // ------------------------------------------------
    // |--30s--|--30s--|--30s--|--30s--| (4 segments in 2 min)
    // |                                | <- Requests slide as segments roll over
    // ------------------------------------------------
    // As new 30-second segments open, the oldest one expires, allowing
    // smoother traffic over time.
    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(2);
        opt.PermitLimit = 20;
        opt.SegmentsPerWindow = 4;  // Divides into 4 rolling segments (every 30 seconds)
    });

    // 3. Token Bucket Limiter (15 tokens max, refill 1 token/sec)
    // ------------------------------------------------
    // Tokens:  15 (Max) — Burst Capacity
    // Refill:  1 token per second (smoother flow)
    // ------------------------------------------------
    // Allows bursts but gradually refills tokens over time.
    // Requests consume tokens, and when depleted, requests are throttled until refilled.
    options.AddTokenBucketLimiter("token", opt =>
    {
        opt.TokenLimit = 15;
        opt.TokensPerPeriod = 1;  // Refill rate
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(1);  // Refill interval
    });

    // 4. Concurrency Limiter (5 concurrent requests, 10 in queue)
    // ------------------------------------------------
    // Concurrent: | | | | | (5 slots)
    // Queue:      | | | | | | | | | | (10 slots)
    // ------------------------------------------------
    // Allows 5 active requests and queues 10 more. If the queue is full,
    // new requests are rejected with 503 or 429 (if overridden).
    options.AddConcurrencyLimiter("concurrency", opt =>
    {
        opt.PermitLimit = 5;  // Max concurrent requests
        opt.QueueLimit = 10;  // Queue limit before rejection
    });
});

var app = builder.Build();

app.UseRateLimiter();

// Map endpoints with rate limiting policies
app.MapGet("/fixed", () => "Fixed window response")
    .RequireRateLimiting("fixed");

app.MapGet("/sliding", () => "Sliding window response")
    .RequireRateLimiting("sliding");

app.MapGet("/token", () => "Token bucket response")
    .RequireRateLimiting("token");

app.MapGet("/concurrency", () => "Concurrency response")
    .RequireRateLimiting("concurrency");

app.Run();
