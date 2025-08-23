#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*
#:package Swashbuckle.AspNetCore.ReDoc@9.0.*
#:property PublishAot=false     

// Redoc Example
// Ideal for viewing documentation (does not support interactive requests)

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseReDoc(options =>
{
    //options.RoutePrefix = "docs"; // change relative path to the UI
    options.DocumentTitle = "Open API - ReDoc";
    options.SpecUrl("/openapi/v1.json");
});
app.MapOpenApi();

app.MapGet("/", () => Results.Redirect("/api-docs/index.html"));
app.Run();