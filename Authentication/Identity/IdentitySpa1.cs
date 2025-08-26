#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.Authentication.Google@10.*-preview*

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

// Step - Setup SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite("Data Source=identity.sql"));

// Step - Add Identity Endpoints
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication();

builder.Services.AddAuthorization();
var app = builder.Build();
app.Run();


public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
}
