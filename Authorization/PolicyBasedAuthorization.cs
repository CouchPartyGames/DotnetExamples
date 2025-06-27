#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// Authorization in Minimal API
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("admin-policy", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationScheme("Cookie")
            .RequireClaim("role", allowedValues: ["admin", "superuser"]);
    });
    
    opts.AddPolicy("user-policy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");
app.MapGet("/auth", () => "Hello World")
    .RequireAuthorization();

app.MapGet("/admin",  () => "Hello Admin")
    .RequireAuthorization("admin-policy");

    // Apply Multiple Policies
app.MapGet("/multiple-policies",   () => "Hello Admin")
    .RequireAuthorization("admin-policy")
    .RequireAuthorization("user-policy");

app.Run();