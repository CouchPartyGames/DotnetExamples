#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Kinde.SDK@1.3.1
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview*

// Kinde Authentication
//
// export Kinde__Authority=""
// export Kinde__ClientId=""
// export Kinde__ClientSecret=""
//
// OIDC Documentation: https://docs.kinde.com/developer-tools/guides/dotnet-open-id-connect/
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder();
builder.Services.Configure<KindeOptions>(
    builder.Configuration.GetSection(KindeOptions.SectionName));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddOpenIdConnect(opts =>
    {
        opts.Authority = builder.Configuration["Kinde:Authority"];
        opts.ClientId = builder.Configuration["Kinde:ClientId"];
        opts.ClientSecret = builder.Configuration["Kinde:ClientSecret"];

        opts.ResponseType = "code";
        opts.MapInboundClaims = false;
    })
    .AddCookie();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/",  () => "Hello World!");
app.MapGet("/user", (HttpContext ctx) =>
{
    return string.Join(", ", ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList());
}).RequireAuthorization();
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});
app.Run();

public sealed class KindeOptions
{
    public const string SectionName = "Kinde";
    
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}