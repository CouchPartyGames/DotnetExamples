#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
var app = builder.Build();
// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


[ApiController]
[Route("[controller]")]
public sealed class HelloController : ControllerBase
{

    [HttpGet]
    public string Get()
    {
        return "Hello World";
    }

    [HttpGet("protected")]
    [Authorize]
    public string Protected()
    {
        return "Protected World";
    }
}
