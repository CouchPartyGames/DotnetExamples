#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Dapper@2.1.66

using Microsoft.AspNetCore.Identity;
using Dapper;
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-9.0

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection());

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<DapperUserStore>();
    
// Inject Authorization/Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
    
var app = builder.Build();

// Ensure Authorization/Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();
app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", () => "Protected")
    .RequireAuthorization();

app.Run();


public sealed class DapperUserStore(IConfiguration configuration) : IUserStore<IdentityUser>
{
    //private IDbConnection CreateConnection() => new SqlConnection();

    public async Task<IdentityResult> CreateAsync()
    {
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync()
    {
        return IdentityResult.Success;
    }
}
