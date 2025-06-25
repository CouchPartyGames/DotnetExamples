#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview*

// Purpose: use openidconnect.net to test oidc

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;     // necessary for AddOpenConnectId

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
var app = builder.Build();
app.Run();