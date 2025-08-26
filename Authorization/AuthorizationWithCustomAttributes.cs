#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();
// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Content("<a href=/protected>Protected</a>", "text/html"));
app.MapGet("/protected", [CustomAuthorize] () => "Protected Resource");
app.Run();


public sealed class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context != null)
        {
            // Auth logic
        }
    }
}