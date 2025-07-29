#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();
// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
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