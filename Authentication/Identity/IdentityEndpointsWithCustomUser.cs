#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@10.*-*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@10.*-*
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Scalar.AspNetCore@2.7.*
#:property PublishAot=false     

// Basic Example of using .NET Core Identity Endpoints
//
// Note: In order to view endpoints via OpenApi/Scalar, AOT must be disabled.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder();

// Step - Setup SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite("Data Source=identity.sql"));

// Step - Add Identity Endpoints
//  Register Api Endpoints
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Step - Setup OpenAPI/Scalar (Optional)
app.MapOpenApi();
app.MapScalarApiReference();

// Step - Map Identity Endpoints/Routes
app.MapIdentityApi<AppUser>();
app.MapGet("/", () => Results.Content("<a href=/scalar/>Scalar/OpenAPI</a> <a href=/me>Protected Page</a>", "text/html"));
app.MapGet("/me", () => "Hello World!").RequireAuthorization();
app.Run();

public sealed class AppUser : IdentityUser
{
    public string Age { get; set; } = string.Empty;
}

public sealed class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options) { }
}