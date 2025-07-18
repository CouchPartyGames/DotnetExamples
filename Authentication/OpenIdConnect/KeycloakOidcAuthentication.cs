#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview.*

using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddOpenIdConnect("keycloak", opts =>
    {
        opts.Authority = "";
        opts.ClientSecret = "";
        opts.ClientId = "";

        opts.Scope.Clear();
        opts.Scope.Add("openid");
        opts.Scope.Add("profile");
        opts.Scope.Add("email");
    });
var app = builder.Build();
app.UseAuthentication();
app.UseAuthentication();

app.MapGet("/", () => "Hello");
app.Run();