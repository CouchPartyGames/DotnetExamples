# OpenAPI Examples for ASP.NET Core

This directory contains comprehensive examples demonstrating various OpenAPI implementations using ASP.NET Core 9+. Each example showcases different aspects of OpenAPI specification and documentation generation.

## Examples Overview

### Basic Examples

| File | Description | Key Features |
|------|-------------|--------------|
| **ControllerWithOpenApi.cs** | Basic Controller-based API with OpenAPI | Uses traditional MVC controllers, demonstrates `EndpointSummary` attribute |
| **MinApiWithOpenApi.cs** | Minimal API with OpenAPI metadata | Shows various HTTP verbs with metadata using extension methods and attributes |
| **OpenApiWithMetadata.cs** | Metadata decoration examples | Compares extension methods vs attributes for endpoint documentation |

### Advanced Configuration

| File | Description | Key Features |
|------|-------------|--------------|
| **OpenApiCustomized.cs** | Custom OpenAPI configuration | Multiple document versions, output caching, custom endpoints |
| **OpenApiWithVersioning.cs** | API versioning with OpenAPI | URL segment versioning, header-based versioning, version sets |
| **OpenApiWithXmlDocumentation.cs** | XML documentation integration | Demonstrates XML comments in OpenAPI spec generation |

### Security & Authentication

| File | Description | Key Features |
|------|-------------|--------------|
| **OpenApiWithAuthentication.cs** | Security schemes integration | Bearer tokens, API keys, OAuth2, OpenID Connect transformers |

### Data Types & Models

| File | Description | Key Features |
|------|-------------|--------------|
| **OpenApiWithEnums.cs** | Enum handling in OpenAPI | Integer enums, string enums, flags enums with JSON converters |

### Documentation UIs

| File | Description | Key Features |
|------|-------------|--------------|
| **OpenApiWithSwaggerUI.cs** | SwaggerUI integration | Traditional Swagger interface for API testing |
| **OpenApiWithScalar.cs** | Basic Scalar UI | Modern API documentation with Scalar |
| **OpenApiWithScalarAdvanced.cs** | Advanced Scalar configuration | Custom themes, titles, HTTP client settings |
| **OpenApiWithRedoc.cs** | ReDoc documentation | Read-only documentation interface |

### Health & Monitoring

| File | Description | Key Features |
|------|-------------|--------------|
| **OpenApiWithHealthChecks.cs** | Health checks integration | Basic health endpoint with optional OpenAPI integration |

## Common Patterns

### Package Dependencies
Most examples use these core packages:
- `Microsoft.AspNetCore.OpenApi` - Core OpenAPI functionality
- `Microsoft.Extensions.ApiDescription.Server` - API description services
- `Scalar.AspNetCore` - Modern API documentation UI

### Standard Implementation
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapOpenApi(); // Usually at /openapi/v1.json
```

### Metadata Approaches
1. **Extension Methods**: `.WithTags()`, `.WithSummary()`, `.WithDescription()`
2. **Attributes**: `[EndpointSummary]`, `[EndpointDescription]`, `[Tags]`
3. **XML Documentation**: Triple-slash comments with `GenerateDocumentationFile=true`

## Accessing Documentation

When running any example:
- **OpenAPI JSON**: `http://localhost:5000/openapi/v1.json`
- **Scalar UI**: `http://localhost:5000/scalar/v1` (if using Scalar)
- **Swagger UI**: `http://localhost:5000/swagger/` (if using SwaggerUI)
- **ReDoc**: `http://localhost:5000/api-docs/` (if using ReDoc)

## Security Examples

The authentication example demonstrates four security schemes:
- **Bearer JWT**: HTTP Bearer token authentication
- **API Key**: Header-based API key authentication  
- **OAuth2**: Authorization code flow with scopes
- **OpenID Connect**: OIDC authentication flow

## Getting Started

1. Choose an example that matches your needs
2. Run with: `dotnet run <filename>`
3. Navigate to the appropriate documentation URL
4. Explore the generated OpenAPI specification and interactive documentation

Each file is self-contained and can be executed independently to demonstrate specific OpenAPI features.