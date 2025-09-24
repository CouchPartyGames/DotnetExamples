#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Client call
// curl -X POST -F "fieldname=@file.txt" http://localhost:5000/upload
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapPost("/upload", async (IFormFile file) =>
{
    string fileName = "myfile.txt";
    await using var stream = File.OpenWrite(fileName);
    await file.CopyToAsync(stream);

    return Results.Ok();
}).DisableAntiforgery();
app.Run();
