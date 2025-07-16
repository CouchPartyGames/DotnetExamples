#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview*

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;     // necessary for AddOpenConnectId

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = "Supabase";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
    {
        opts.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        opts.Cookie.IsEssential = true;
    })
    .AddOpenIdConnect("Supabase", opts =>
    {
        opts.ClientId = Environment.GetEnvironmentVariable("SUPABASE_CLIENT_ID");
        opts.ClientSecret = Environment.GetEnvironmentVariable("SUPABASE_CLIENT_SECRET");
        

    });
    
    
    
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", () => "Secret").RequireAuthorization();
app.Run();