# Basic Authentication Example

This project demonstrates HTTP Basic Authentication implementation in ASP.NET Core with both server and client components.

## Overview

HTTP Basic Authentication passes credentials via the `Authorization` header using the format:
```
Authorization: Basic <base64(username:password)>
```

**⚠️ Security Note:** Use HTTPS in production to avoid credentials being transmitted in plaintext.

## Files

### BasicAuthentication.cs
A complete ASP.NET Core web server implementing Basic Authentication:

- **Custom Authentication Handler**: `BasicAuthenticationHandler` processes Basic auth headers
- **Extension Methods**: Fluent configuration with `AddBasic()` method
- **Protected Endpoints**: 
  - `/` - Public endpoint
  - `/user` - Protected endpoint requiring authentication
- **Hardcoded Credentials**: `admin:secret` (demo only - use secure credential storage in production)

Key components:
- `BasicAuthenticationDefaults` - Constants to avoid magic strings
- `BasicAuthenticationOptions` - Configuration options
- `BasicAuthenticationHandler` - Core authentication logic

### BasicAuthenticationClient.cs
HTTP client that tests the Basic Authentication server:

- Tests public endpoint without authentication
- Tests protected endpoint without authentication (expects 401)
- Tests with invalid credentials (`testuser:testpass`)
- Tests with valid credentials (`admin:secret`)

## Usage

1. **Run the server:**
   ```bash
   dotnet run BasicAuthentication.cs
   ```
   Server runs on `http://localhost:5000`

2. **Run the client:**
   ```bash
   dotnet run BasicAuthenticationClient.cs
   ```

## Authentication Flow

1. Client sends request to protected endpoint
2. Server checks for `Authorization` header
3. Server validates "Basic" prefix and decodes base64 credentials
4. Server validates username:password against stored credentials
5. On success, creates `ClaimsPrincipal` with user identity and roles
6. Returns `AuthenticationTicket` for successful authentication

## Expected Results

- Public endpoint (`/`): Returns "Hello World!" without authentication
- Protected endpoint (`/user`) without auth: Returns 401 Unauthorized
- Protected endpoint with invalid credentials: Returns 401 Unauthorized  
- Protected endpoint with valid credentials (`admin:secret`): Returns "Hello World!"