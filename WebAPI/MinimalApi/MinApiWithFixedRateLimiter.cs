#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRateLimiter(opts =>
{
    opts.AddFixedWindowLimiter("fixed-12secs", rateOps =>
    {
        rateOps.PermitLimit = 4;
        rateOps.Window = TimeSpan.FromSeconds(12);
        //rateOps.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        rateOps.QueueLimit = 2;
    });
});
var app = builder.Build();
app.UseRateLimiter();
app.MapGet("/", () => "Hello World")
    .RequireRateLimiting("fixed-12secs");

app.Run();