#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Purpose: Validate User Input on Minimal API
//
// https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-10.0
// https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
var app = builder.Build();

// curl -v -X POST -data '{"productId":"xyz","name":"jete"}'  http://localhost:5000/products
// Validate product and name
app.MapPost("/products",
    ([MinLength(5, ErrorMessage = "Product ID must be 5 characters")] string productId, [Required] string name)
        => TypedResults.Ok(productId));

// Validate the id string
app.MapGet("/products/{id}", ([MinLength(5, ErrorMessage="Product Id must be 5 characters")] string id) =>
{
    return TypedResults.Ok($"product: {id}");
});

// Don't Validate
app.MapGet("/no-validation/{id}", ([MinLength(5, ErrorMessage = "Product Id must be 5 characters")] string id) =>
{
    return TypedResults.Ok($"product: {id}");
}).DisableValidation();

app.Run();