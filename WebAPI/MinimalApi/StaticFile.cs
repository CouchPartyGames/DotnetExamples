#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web

// mkdir wwwroot
// touch wwwroot/hello.html
var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapGet("/download", () => Results.File("otherfile.html"));
app.UseStaticFiles();
app.Run("http://localhost:5001");
