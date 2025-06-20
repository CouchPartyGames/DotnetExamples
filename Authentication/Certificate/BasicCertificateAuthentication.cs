#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.Certificate@10.0.0-preview*

// Certificate Authentication
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-10.0
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(opts =>
    {
        
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", () => "Secret").RequireAuthorization();
app.Run();
