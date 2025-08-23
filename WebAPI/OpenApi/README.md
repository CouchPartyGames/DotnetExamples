# OpenAPI Examples

This directory contains comprehensive examples demonstrating how to implement OpenAPI (formerly Swagger) in ASP.NET Core applications using both Controllers and Minimal APIs.

## Overview

OpenAPI is a specification for documenting REST APIs. These examples show how to:
- Generate OpenAPI specifications automatically from your code
- Customize OpenAPI documents with metadata and security schemes
- Display API documentation using various UI tools
- Implement versioning and health checks with OpenAPI
- Use different authentication schemes in your API documentation

## Examples

### Basic OpenAPI Implementation

#### `ControllerWithOpenApi.cs`
- **Purpose**: Demonstrates OpenAPI with traditional Controller-based APIs
- **Features**: 
  - Basic OpenAPI setup with `AddOpenApi()`
  - Controller with `[ApiController]` attribute
  - OpenAPI endpoint mapping at `/openapi/v1.json`
- **Use Case**: When you prefer controller-based architecture with OpenAPI

#### `MinApiWithOpenApi.cs`
- **Purpose**: Shows OpenAPI integration with Minimal APIs
- **Features**:
  - All HTTP methods (GET, POST, PUT, DELETE, PATCH)
  - OpenAPI metadata using fluent API methods
  - Tags, summaries, and descriptions for endpoints
- **Use Case**: Modern, lightweight API development with OpenAPI

### Advanced OpenAPI Features

#### `OpenApiCustomized.cs`
- **Purpose**: Demonstrates advanced OpenAPI configuration
- **Features**:
  - Multiple OpenAPI document versions (v1, v2)
  - Custom OpenAPI version specification (3.1)
  - Output caching for OpenAPI responses
  - Custom endpoint paths
- **Use Case**: When you need multiple API versions or custom caching

#### `OpenApiWithAuthentication.cs`
- **Purpose**: Shows how to document various authentication schemes
- **Features**:
  - JWT Bearer token authentication
  - API Key authentication
  - OAuth2 authentication
  - OpenID Connect authentication
  - Custom document transformers
- **Use Case**: APIs requiring authentication with proper OpenAPI documentation

#### `OpenApiWithEnums.cs`
- **Purpose**: Demonstrates enum handling in OpenAPI
- **Features**:
  - Standard enums
  - String-based enums with JSON conversion
  - Flag enums for bitwise combinations
  - Scalar integration for interactive documentation
- **Use Case**: APIs that use enums as parameters or return values

#### `OpenApiWithHealthChecks.cs`
- **Purpose**: Integrates health checks with OpenAPI
- **Features**:
  - Health check endpoint registration
  - OpenAPI integration with health monitoring
  - Scalar documentation integration
- **Use Case**: Production APIs requiring health monitoring

#### `OpenApiWithMetadata.cs`
- **Purpose**: Shows different ways to add metadata to endpoints
- **Features**:
  - Fluent API methods for metadata
  - Attribute-based metadata
  - Tags, summaries, and descriptions
- **Use Case**: When you need rich API documentation

### Documentation UI Options

#### `OpenApiWithScalar.cs`
- **Purpose**: Basic Scalar integration for interactive API documentation
- **Features**:
  - Scalar UI for testing API endpoints
  - All HTTP methods demonstrated
  - Redirect to Scalar interface
- **Use Case**: Interactive API documentation and testing

#### `OpenApiWithScalarAdvanced.cs`
- **Purpose**: Advanced Scalar configuration
- **Features**:
  - Custom themes (BluePlanet)
  - Custom titles
  - HTTP client configuration
  - Enhanced user experience
- **Use Case**: Production-ready API documentation with custom branding

#### `OpenApiWithSwaggerUI.cs`
- **Purpose**: Traditional Swagger UI integration
- **Features**:
  - Classic Swagger interface
  - Custom endpoint configuration
  - Redirect to Swagger UI
- **Use Case**: When you prefer the traditional Swagger interface

#### `OpenApiWithRedoc.cs`
- **Purpose**: ReDoc integration for read-only documentation
- **Features**:
  - Clean, responsive documentation view
  - Custom document titles
  - No interactive testing (read-only)
- **Use Case**: Public API documentation where you don't want users testing endpoints

#### `OpenApiWithVersioning.cs`
- **Purpose**: API versioning with OpenAPI
- **Features**:
  - Multiple API versions (v1, v2)
  - URL segment and header-based versioning
  - Version-specific endpoint mapping
  - Scalar integration with versioning
- **Use Case**: APIs that need to maintain multiple versions simultaneously

## Getting Started

### Prerequisites
- .NET 10.0 or later
- ASP.NET Core

### Basic Setup
1. Add the required packages:
   ```bash
   dotnet add package Microsoft.AspNetCore.OpenApi
   dotnet add package Microsoft.Extensions.ApiDescription.Server
   ```

2. Register OpenAPI services:
   ```csharp
   builder.Services.AddOpenApi();
   ```

3. Map OpenAPI endpoints:
   ```csharp
   app.MapOpenApi(); // Available at /openapi/v1.json
   ```

### Running the Examples
Each example can be run independently:

```bash
dotnet run --project ControllerWithOpenApi.cs
dotnet run --project MinApiWithOpenApi.cs
# etc.
```

## Key Benefits

- **Automatic Documentation**: Generate API documentation from your code
- **Interactive Testing**: Test endpoints directly from the documentation
- **Multiple UI Options**: Choose from Scalar, Swagger UI, or ReDoc
- **Security Integration**: Document authentication and authorization schemes
- **Versioning Support**: Handle multiple API versions seamlessly
- **Health Monitoring**: Integrate health checks with API documentation

## Best Practices

1. **Use Descriptive Metadata**: Add meaningful summaries and descriptions to your endpoints
2. **Implement Security Schemes**: Document authentication requirements properly
3. **Version Your APIs**: Use semantic versioning for long-term API maintenance
4. **Cache OpenAPI Responses**: Implement caching for better performance
5. **Choose the Right UI**: Select documentation UI based on your audience needs

## Resources

- [ASP.NET Core OpenAPI Documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Scalar Documentation](https://docs.scalar.com/)
- [Swagger UI](https://swagger.io/tools/swagger-ui/)
- [ReDoc](https://redocly.github.io/redoc/)

## Contributing

Feel free to add more examples or improve existing ones. Each example should demonstrate a specific OpenAPI feature or use case clearly.
