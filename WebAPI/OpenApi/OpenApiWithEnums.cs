#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*
#:package Scalar.AspNetCore@2.4.22
#:property PublishAot=false

// Simple OpenAPI Example using Enums
//
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-9.0&tabs=minimal-apis
using System.Text.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-9.0#use-scalar-for-interactive-api-documentation
// http://localhost:5000/scalar/v1
app.MapScalarApiReference();

app.MapGet("/", () => "Hello World!");
app.MapGet("/test1", (Days dayOfTheWeek) =>
{
    return TypedResults.Ok($"The day of the week is {dayOfTheWeek}");
});
app.MapGet("/test2", (DayOfTheWeekAsString dayOfTheWeek) =>
{
    return TypedResults.Ok($"The day of the week is {dayOfTheWeek}");
});
app.MapGet("/test3", (PizzaToppings topping) =>
{
    return TypedResults.Ok($"Your toppping is {topping}");
});
app.Run();


public enum Days
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}

// Step
// You can use strings to set the day of week
// If you send a value not in the enum, the endpoint will respond w/ status code 400
[JsonConverter(typeof(JsonStringEnumConverter<DayOfTheWeekAsString>))]
public enum DayOfTheWeekAsString
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday
}

// Step - Flags Allows for Bitwise Combinations
// You can use both integers and strings to set your topping
// If you send an invalid string, the endpoint will respond w/ status code 400
// If you send an invalid integer, the endpoint will repond w/ status code 200 and the integer
[Flags, JsonConverter(typeof(JsonStringEnumConverter<PizzaToppings>))]
public enum PizzaToppings { Pepperoni = 1, Sausage = 2, Mushrooms = 4, Anchovies = 8 }