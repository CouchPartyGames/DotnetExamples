#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRateLimiter(opts =>
{
   opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
var app = builder.Build();
app.UseRateLimiter();
app.MapGet("/", () => "Hello World")
   .RequireRateLimiting("global");
app.Run();