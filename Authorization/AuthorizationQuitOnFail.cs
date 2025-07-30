#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// https://timdeschryver.dev/blog/nice-to-knows-when-implementing-policy-based-authorization-in-net#all-policies-handlers-are-always-evaluated

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    // Step - Tell Controllers
    options.InvokeHandlersAfterFailure = false;
});
var app = builder.Build();

// Note: Authentication middleware must come before Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.Run();
