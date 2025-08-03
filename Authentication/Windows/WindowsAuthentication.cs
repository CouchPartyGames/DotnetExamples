#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.Negotiate@10.0.0-preview*

// https://www.bizstream.com/blog/how-to-set-up-windows-authentication-with-net-5/
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.Run();