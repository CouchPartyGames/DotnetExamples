#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Scalar.AspNetCore@2.4.22
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
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Step - Register OpenAPI (Optional)
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Step - Setup OpenAPI/Scalar (Optional)
app.MapOpenApi();
app.MapScalarApiReference();

// Step - Map Identity Endpoints/Routes
app.MapIdentityApi<IdentityUser>();
app.MapGet("/", () => Results.Content("<a href=/scalar/>Identity Documentation</a><a href=/me>Protected Page</a>", "text/html"));
app.MapGet("/me", () => "Hello World!").RequireAuthorization();
app.Run();

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options) { }
}
