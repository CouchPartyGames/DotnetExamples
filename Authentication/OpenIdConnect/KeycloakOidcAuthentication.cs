#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.OpenIdConnect@10.0.0-preview.*

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder();
builder.Services.Configure<KindeOptions>(
    builder.Configuration.GetSection(KeycloakOptions.SectionName));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddOpenIdConnect(opts =>
    {
        opts.Authority = builder.Configuration["Keycloak:Authority"];
        opts.ClientSecret = builder.Configuration["Keycloak:ClientSecret"];
        opts.ClientId = builder.Configuration["Keycloak:ClientId"];
        
        opts.ResponseType = "code";
        opts.MapInboundClaims = false;

        opts.Scope.Clear();
        opts.Scope.Add("openid");
        opts.Scope.Add("profile");
        opts.Scope.Add("email");
    })
    .AddCookie();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthentication();

app.MapGet("/", () => "Hello");
app.MapGet("/user", (HttpContext ctx) =>
{
    return string.Join(", ", ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList());
}).RequireAuthorization();
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
}).RequireAuthorization();
app.Run();

public sealed class KeycloakOptions
{
    public const string SectionName = "Keycloak";
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public required string  ClientSecret { get; init; }
}