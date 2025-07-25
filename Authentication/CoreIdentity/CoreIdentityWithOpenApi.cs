#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.*
#:package Microsoft.EntityFrameworkCore.InMemory@9.*
#:package Microsoft.AspNetCore.OpenApi@10.0.0-preview*
#:package Scalar.AspNetCore@2.4.22
#:property PublishAot=false     
// Must disable AOT

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
    
// Inject OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
    
var app = builder.Build();

// Ensure Authorization/Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Add OpenAPI 
// http://localhost:5000/openapi/v1.json
app.MapOpenApi();

// Add Scalar UI
// http://localhost:5000/scalar/v1
app.MapScalarApiReference();

app.MapIdentityApi<IdentityUser>();
app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", () => "Protected")
    .RequireAuthorization();

app.Run();


public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}
