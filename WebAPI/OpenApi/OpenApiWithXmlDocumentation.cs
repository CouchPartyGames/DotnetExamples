#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*
#:package Scalar.AspNetCore@2.7.*
#:property GenerateDocumentationFile=true
#:property PublishAot=false     


// Example of using OpenAPI Metadata with XML Documentation
//
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/openapi-comments?view=aspnetcore-10.0
//
// Requirements: Add the following options to your csproj file 
/*
	<PropertyGroup>
		<GenerateDocumentationFile>true</GenerateDocumentationFile>
	</PropertyGroup>
*/
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

    // Inject OpenAPI
builder.Services.AddOpenApi();
var app = builder.Build();

    // http://localhost:5000/openapi/v1.json
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/{id}", EndpointExtensions.GetProjectBoardById);
app.Run();




public static class EndpointExtensions {

	/// <summary>
	/// Retrieves a specific project board by ID.
	/// </summary>
	/// <param name="id">The ID of the project board to retrieve.</param>
	/// <returns>The requested project board.</returns>
	/// <response code="200">Returns the requested project board.</response>
	/// <response code="404">If the project board is not found.</response>
	public static IResult GetProjectBoardById(int id)
	{
		List<Board> boards = new List<Board>() {
			new Board(1, "Hello"),
			new Board(2, "World"),
			new Board(3, "Test")
		};
		var board = boards.FirstOrDefault(b => b.Id == id);
		if (board == null)
		{
			return Results.NotFound();
		}
		return Results.Ok(board);
	}
}

public record Board(int Id, string Name);

