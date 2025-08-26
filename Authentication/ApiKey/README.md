# API Key Authentication Examples

This directory contains various implementations of API Key authentication for ASP.NET Core applications, demonstrating different approaches for securing endpoints.

## Files Overview

### Authentication Implementations

**ApiKeyAuthenticationWithMiddleware.cs**
- Implements API Key authentication using custom middleware
- Protects ALL routes in the application
- Uses timing-attack resistant comparison with `CryptographicOperations.FixedTimeEquals`
- Accepts API keys via `X-Api-Key` header
- Note: Contains incomplete code in the timing attack prevention method

**ApiKeyAuthenticationWithAttribute.cs**  
- Uses MVC Service Filter Attribute approach for API Key authentication
- Protects specific endpoints by applying `[ApiKey]` attribute
- Implements `IAuthorizationFilter` for authorization logic
- Suitable for MVC controllers and actions
- Uses timing-attack resistant comparison

**ApiKeyAuthenticationWithEndpointFilter.cs**
- Demonstrates API Key authentication using Minimal API Endpoint Filters
- Shows both strongly-typed filter class and inline delegate approaches
- Protects specific routes by adding the filter to individual endpoints
- Uses timing-attack resistant comparison
- Accepts API keys via `X-Api-Key` header

**ApiKeyAuthenticationWithSecurityKey.cs**
- Minimal example using the AspNetCore.SecurityKey NuGet package
- Placeholder implementation showing external library integration

### API Key Generation Utilities

**GenerateApiKey.cs**
- Generates cryptographically secure API keys using `RandomNumberGenerator`
- Creates 32-byte keys encoded as Base64 strings
- Uses strong cryptographic random values

**GenerateApiKeyWithGuid.cs**
- Alternative API key generation using `Guid.CreateVersion7()`
- Provides both GUID and Base64 string formats
- Version 7 GUIDs are optimized for database indexing

## Security Features

All implementations include:
- **Timing Attack Prevention**: Uses `CryptographicOperations.FixedTimeEquals` to prevent timing side-channel attacks
- **Cryptographically Secure Generation**: Uses proper random number generators for key creation
- **Header-based Authentication**: Accepts API keys via HTTP headers (typically `X-Api-Key`)

## Usage Patterns

- **Middleware**: Use for protecting all routes in your application
- **Attribute**: Use with MVC for protecting specific controllers or actions  
- **Endpoint Filter**: Use with Minimal APIs for protecting specific endpoints
- **External Library**: Use AspNetCore.SecurityKey package for ready-made solutions

## Testing

Test the implementations using curl:
```bash
curl -v -H "X-Api-Key: your-api-key" http://localhost:5000/protected
```