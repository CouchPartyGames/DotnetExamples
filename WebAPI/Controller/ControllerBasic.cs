#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// A Simple Hello World example using Controllers
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();


[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    // GET: api/hello
    [HttpGet]
    public ActionResult GetHello()
    {
        return Ok("Hello World!");
    }
}