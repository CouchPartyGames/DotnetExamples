#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.InMemory@9.0.*
#:package Microsoft.EntityFrameworkCore.Tools@9.0.*
#:package Microsoft.AspNetCore.Identity@2.3.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Scalar.AspNetCore@2.4.22
#:property PublishAot=false     

// Simple Example of using .NET Core Identity for Single Page Application (SPA
//
// Notice: Disable AOT for Scalar to properly render OpenAPI endpoints
// Notice: You don't have to inject Authentication dependencies

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder();

    // Step - Setup In Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

    // Step - Add Identity Endpoints
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();

    // Step - Map Identity Endpoints/Routes
app.MapIdentityApi<IdentityUser>();

app.MapGet("/", () => Results.Redirect("/scalar"))
    .Produces(302);
app.Run();

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}
