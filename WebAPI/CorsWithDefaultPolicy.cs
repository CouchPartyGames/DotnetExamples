#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// When you want the same CORS policy applied to all endpoints without specifying a policy name
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("https://myapp.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
var app = builder.Build();

// Step - Apply Specific Cors Middleware
//  Will use the default cors policy
//  Order is important, UseCors() must be called before UseResponseCache()
app.UseCors();

app.MapGet("/", () => "Hello World");
app.MapGet("/override", () => "Hello override")
    .RequireCors(CorsPolicyExtensions.OverridePolicyName);

var apiGroup = app.MapGroup("/api")
    .RequireCors(CorsPolicyExtensions.OverridePolicyName);

apiGroup.MapGet("/override", () => "Hello override with map group");

app.Run();


public static class CorsPolicyExtensions {
    
    public const string OverridePolicyName = "OverridePolicy";
    public static CorsPolicy OverridePolicy() => 
        new CorsPolicyBuilder()
            .WithOrigins("https://my.site")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .Build();
}
