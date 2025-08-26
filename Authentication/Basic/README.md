# Basic Authentication Example

This project demonstrates HTTP Basic Authentication implementation in ASP.NET Core.

## Overview

HTTP Basic Authentication uses an Authorization header to pass credentials to the server:
- Format: `Authorization: Basic <credentials>`
- `<credentials>` is base64-encoded `username:password`
- **Important**: Use HTTPS to secure credentials transmission

## Implementation

The example includes:

### Authentication Setup
- Custom `BasicAuthenticationHandler` that validates Authorization headers
- Extension methods for easy service registration
- Configurable options through `BasicAuthenticationOptions`

### Key Components

1. **BasicAuthenticationDefaults** (`BasicAuthentication.cs:38-41`)
   - Defines default scheme name to avoid magic strings

2. **BasicAuthenticationExtensions** (`BasicAuthentication.cs:44-64`)
   - Extension methods for service registration
   - Multiple overloads for flexibility

3. **BasicAuthenticationOptions** (`BasicAuthentication.cs:68-71`)
   - Configuration options for the authentication scheme

4. **BasicAuthenticationHandler** (`BasicAuthentication.cs:75-121`)
   - Core authentication logic
   - Validates Authorization header format
   - Decodes base64 credentials
   - Creates claims and authentication ticket

### Endpoints

- `/` - Public endpoint
- `/user` - Protected endpoint requiring authentication

## Usage

The handler currently accepts any credentials and creates a fixed identity with:
- NameIdentifier: "Bob"
- Role: "Admin"

In production, replace the hardcoded authentication logic with actual user validation.