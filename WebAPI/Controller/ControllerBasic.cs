#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false     

// A Simple Hello World example using Controllers
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();


[Route("api/[controller]")]
[ApiController]
public sealed class HelloController : ControllerBase
{
    // GET: api/hello
    [HttpGet]
    public ActionResult GetHello()
    {
        return Ok("Hello World!");
    }

    // GET: api/hello/3
    [HttpGet("{id}")]
    public ActionResult GetById(int id)
    {
        return NotFound();
    }
}