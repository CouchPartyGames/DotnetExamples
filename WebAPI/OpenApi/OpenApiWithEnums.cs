#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.0.0-preview*
#:package Microsoft.Extensions.ApiDescription.Server@10.0.0-preview*

//
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-9.0&tabs=minimal-apis

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi();
app.Run();


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

[Flags, JsonConverter(typeof(JsonStringEnumConverter<PizzaToppings>))]
public enum PizzaToppings { Pepperoni = 1, Sausage = 2, Mushrooms = 4, Anchovies = 8 }