#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@10.0.*
#:package Microsoft.AspNetCore.OpenApi@10.0.*
#:package Scalar.AspNetCore@2.10.*
#:property PublishAot=false     

// Simple Example of using .NET Core Identity for Single Page Application (SPA
//
// Add Logout Functionality since Core Identity doesn't provide it.
// Notice: Disable AOT for Scalar to properly render OpenAPI endpoints

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder();

    // Step - Setup In Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite("Data Source=identity.sql"));

    // Step - Add Identity Endpoints
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication();
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

    // Step - Add Logout
    // But is this really necessary/depends
app.MapPost("/logout", async(SignInManager<IdentityUser> manager) =>
{
    await manager.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

app.Run();

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}
