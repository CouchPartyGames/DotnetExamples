#!/usr/bin/env -S dotnet run
#:package Dapper@2.1.66
#:package Npgsql@9.0.3

using Npgsql;
using Dapper;

var connectionString = "Server=127.0.0.1;Port=5432;Database=DapperDB;User Id=postgres;Password=z;";
// Connect to the database
using (var connection = new NpgsqlConnection(connectionString))
{
    // Create a query that retrieves all authors   
    var sql = "SELECT * FROM Authors";     
    // Use the Query method to execute the query and return a list of objects
    var test = await connection.QueryAsync(sql);
}

public class Author
{
    public required string Id { get; set; }
}