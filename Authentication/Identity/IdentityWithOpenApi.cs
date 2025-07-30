#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.InMemory@9.0.*
#:package Microsoft.EntityFrameworkCore.Tools@9.0.*
#:package Microsoft.AspNetCore.Identity@2.3.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Scalar.AspNetCore@2.4.22
#:property PublishAot=false     
// Must disable AOT

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication()
    .AddCookie("Identity.Bearer");
builder.Services.AddAuthorization();

// Add Database Dependency
builder.Services.AddDbContext<AppDbContext>(
    opts => opts.UseInMemoryDatabase("MyTestDatabase"));

// Add Identity Core
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();
//builder.Services.AddIdentityApiEndpoints<IdentityUser>()
//    .AddEntityFrameworkStores<AppDbContext>();
    //.AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(opts => {
	opts.AccessDeniedPath = "/account/denied";
});
builder.Services.AddOpenApi();

var app = builder.Build();

    // Authentication Middleware
app.UseAuthentication();
app.UseAuthorization();

    // Add Identity Endpoints
app.MapIdentityApi<IdentityUser>();
app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"))
    .Produces(302);

app.Run();

public sealed class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) {}

}