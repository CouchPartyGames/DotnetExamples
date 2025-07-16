#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview*

// Purpose: use openidconnect.net to test oidc
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-oidc-web-authentication?view=aspnetcore-9.0
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;     // necessary for AddOpenConnectId

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddOpenIdConnect("oidc", opts =>
    {
        opts.Authority = "https://demo.identityserver.io/";
        opts.ClientId = "PlaygroundAuthentication";
        opts.ClientSecret = "PlaygroundAuthenticationClientSecret";
        
        opts.ResponseType = "code";
    });
var app = builder.Build();
app.Run();