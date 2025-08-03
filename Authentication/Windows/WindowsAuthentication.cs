#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.Negotiate@10.0.0-preview*

// Example of Authentication using Windows (Windows Only)
// 
// Works with Microsoft Edge, Google Chrome on Windows 10/11
// Failed on Mozilla Firefox on any version of Windows
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!");
app.MapGet("/user", [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)] (HttpContext context) =>
    "User ID: " + context.User.Identity?.Name);


app.Run();