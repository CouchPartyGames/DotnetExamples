#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Asp.Versioning.Http@8.1.0

// Very Simple Minimal API
// https://github.com/dotnet/aspnet-api-versioning/wiki/Versioning-by-Media-Type
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Step - Register Api Versioning
builder.Services.AddApiVersioning(options =>
{
    // Set Default Version
    options.DefaultApiVersion = new ApiVersion(1);
    // Add a Header that sets a header in the response
    options.ReportApiVersions = true;
    // When a client doesn't provide a version, tell the api to assume the default version
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Where to look for the version
    options.ApiVersionReader = ApiVersionReader.Combine(
        new MediaTypeApiVersionReader()
        );
});

var app = builder.Build();

var hello = app.NewVersionedApi();
var v1 = hello.MapGroup( "/helloworld" ).HasApiVersion(1);
var v2 = hello.MapGroup( "/helloworld" ).HasApiVersion(2);

app.Run();
