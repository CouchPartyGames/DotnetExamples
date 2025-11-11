#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.*

// Supabase Authentication
//
//
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(opts =>
    {
        opts.ClientId = builder.Configuration["Supabase:ClientId"];
        opts.ClientSecret = builder.Configuration["Supabase:ClientSecret"];
    });
    
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/user", (HttpContext ctx) =>
{
    return string.Join(", ", ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList() );
}).RequireAuthorization();
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
}).RequireAuthentication();

app.Run();


public sealed class SupabaseOptions
{
    public const string Section = "supabase";
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}