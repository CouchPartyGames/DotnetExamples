#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package ModelContextProtocol@0.3.0-preview.*
#:package ModelContextProtocol.AspNetCore@0.3.0-preview.*

// https://github.com/modelcontextprotocol/csharp-sdk/blob/main/README.md
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Step - Register MCP Server
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<TimeTool>();

var app = builder.Build();

// Step - Add MCP
app.MapMcp();
app.Run();


[McpServerToolType]
public sealed class TimeTool
{
    [McpServerTool, Description("The type of time to wait for the server to start")]
    public static string GetCurrentTime(string city) => 
        $"It is {DateTime.Now.Hour:00} in {city}";
}
