#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Kinde.SDK@1.3.1
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview*

// OIDC Documentation: https://docs.kinde.com/developer-tools/guides/dotnet-open-id-connect/
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddOpenIdConnect();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});
app.Run();
