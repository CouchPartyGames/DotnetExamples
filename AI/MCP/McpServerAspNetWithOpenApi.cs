#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package ModelContextProtocol@0.3.*-*
#:package ModelContextProtocol.AspNetCore@0.3.*-*
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*
#:package Microsoft.OpenApi@2.1.0
#:property PublishAot=false 

// Setup MCP Server with OpenAPI
//
// https://github.com/modelcontextprotocol/csharp-sdk/blob/main/README.md
// https://techcommunity.microsoft.com/blog/azuredevcommunityblog/swagger-auto-generation-on-mcp-server/4432196
using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;

var builder = WebApplication.CreateBuilder(args);

// Step - Register MCP Server
builder.Services.AddMcpServer()
    .WithHttpTransport(opts => opts.Stateless = true)
    .WithTools<TimeTool>();

// Step - Add Access to HTTP Context
builder.Services.AddHttpContextAccessor();

// Step - Register Open API
//  http://localhost:5000/openapi/v1.json
builder.Services.AddOpenApi(opts =>
{
    // For openapi.json
    opts.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
    opts.AddDocumentTransformer<McpDocumentTransformer>();
});

var app = builder.Build();

// Step - Add MCP
//  
app.MapMcp("/mcp");

// Step - Add Endpoint for OpenAPI
app.MapOpenApi();

app.Run();


[McpServerToolType]
public sealed class TimeTool
{
    [McpServerTool, Description("The type of time to wait for the server to start")]
    public static string GetCurrentTime(string city) => 
        $"It is {DateTime.Now.Hour:00} in {city}";
}

public class McpDocumentTransformer(IHttpContextAccessor accessor) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "MCP server",
            Version = "1.0.0",
            Description = "A simple MCP server."
        };
        
        document.Servers =
        [
            new OpenApiServer
            {
                Url = accessor.HttpContext != null
                    ? $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}/"
                    : "http://localhost:8080/"
            }
        ];

        var pathItem = new OpenApiPathItem {
            Operations = new()
            {
                [HttpMethod.Post] = new OpenApiOperation
                {
                    Summary = "MCP endpoint",
                    Description = "Returns all pets from the system that the user has access to",
                    /* error CS0246: The type or namespace name 'OpenApiString' could not be found (are you missing a using directive or an assembly reference?
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["x-ms-agentic-protocol"] = new OpenApiString("mcp-streamable-1.0")
                    }, */
                    OperationId = "InvokeMCP",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "Success"
                        }
                    }
                }
            }
        };

        document.Paths ??= [];
        document.Paths.Add("/mcp", pathItem);

        return Task.CompletedTask;
    }
}