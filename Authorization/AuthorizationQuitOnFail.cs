#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// https://timdeschryver.dev/blog/nice-to-knows-when-implementing-policy-based-authorization-in-net#all-policies-handlers-are-always-evaluated

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.InvokeHandlersAfterFailure = false;
});
var app = builder.Build();
