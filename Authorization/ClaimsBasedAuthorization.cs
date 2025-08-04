#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Authorization in Minimal API using Policy Claims
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opts =>
{
    // Step - Create a Policy with Claim
    opts.AddPolicy("Normal", policyBuilder =>
    {
        policyBuilder.RequireClaim("SkillType", "Normal");
    });
    
    // Step - Create a Policy with Claim
    opts.AddPolicy("Advanced", policyBuilder =>
    {
        policyBuilder.RequireClaim("SkillType", "Advanced");
    });
});

var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World");

// Step - Apply Policy
app.MapGet("/advanced", () => "Hello World")
    .RequireAuthorization("Advanced");

// Step - Apply Policy
app.MapGet("/normal", () => "Hello World")
    .RequireAuthorization("Normal");

app.Run();