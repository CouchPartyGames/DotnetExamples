#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.0.0-preview*

// A Simple Hello World example using Controllers
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Inject OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// http://localhost:5000/openapi/v1.json
app.MapOpenApi();
app.MapControllers();

app.Run();


[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    // GET: api/hello
    [EndpointSummary("This is a second summary from OpenApi attributes.")]
    [HttpGet]
    public ActionResult GetHello()
    {
        return Ok("Hello World!");
    }
}