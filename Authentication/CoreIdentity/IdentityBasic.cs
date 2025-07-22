#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@9.0.*
#:package Microsoft.EntityFrameworkCore.Tools@9.0.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorization();
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();
builder.Services.AddDbContext<AppDbContext>(opts =>
{ 
    opts.UseSqlite("Data Source=identity.db"); 
});

var app = builder.Build();

// Authentication Middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.Run();


public sealed class User : IdentityUser
{
}

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) {}
}