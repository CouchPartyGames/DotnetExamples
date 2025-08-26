#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package HotChocolate.AspNetCore@16.0.0-p.6.12

// Simple GraphQL Server

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGraphQL()
    .AddQueryType<MyQuery>();
var app = builder.Build();
app.MapGraphQL();
/*app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
})*/
app.Run();


public class MyQuery
{
    public static Book GetBook()
        => new Book("C# in depth.", new Author("Jon Skeet"));
}

public record Author(string Name);

public record Book(string Title, Author Author);