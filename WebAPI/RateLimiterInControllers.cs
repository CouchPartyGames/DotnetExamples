#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.UseRateLimiter();
app.MapControllers();
app.Run();

public class WeatherController : BaseController
{
    [HttpGet]
    [EnableRateLimiting("fixed")]
    public IEnumerable Get()
    {
        
    }
}