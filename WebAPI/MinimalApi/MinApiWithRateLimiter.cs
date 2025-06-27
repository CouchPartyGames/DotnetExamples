#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRateLimiter(opts =>
{
   opts.GlobalLimiter =
});
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.Run();