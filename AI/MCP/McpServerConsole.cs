#!/usr/bin/env dotnet
#:package ModelContextProtocol@0.3.0-preview.*
#:package Microsoft.Extensions.Hosting@10.0.0-preview*

// https://github.com/modelcontextprotocol/csharp-sdk/blob/main/README.md
/*
  {
     "servers": {
       "MyMcpServer": {
         "type": "stdio",
         "command": "dotnet",
         "args": ["run", "C:\\\\path\\\\to\\\\your\\\\mcp-server.cs"]
       }
     }
   }
 */

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Step - Register MCP Server
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello {message}";
}