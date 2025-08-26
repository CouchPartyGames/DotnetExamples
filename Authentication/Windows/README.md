# Windows Authentication Example

A minimal ASP.NET Core web application demonstrating Windows Authentication using the Negotiate authentication scheme.

## Overview

This example shows how to implement Windows Authentication in ASP.NET Core using the `Microsoft.AspNetCore.Authentication.Negotiate` package. The application authenticates users against their Windows credentials automatically when accessing protected endpoints.

## Features

- **Automatic Windows Authentication**: Uses the Negotiate authentication scheme for seamless Windows login
- **Two Endpoints**:
  - `/` - Public endpoint returning "Hello World!"
  - `/user` - Protected endpoint displaying the authenticated user's Windows username

## Requirements

- Windows operating system
- .NET SDK
- Microsoft.AspNetCore.Authentication.Negotiate package (version 10.0.0-preview or later)

## Browser Compatibility

| Browser | Windows 10/11 | Status |
|---------|---------------|---------|
| Microsoft Edge | ✅ | Supported |
| Google Chrome | ✅ | Supported |
| Mozilla Firefox | ❌ | Not supported |

## Usage

1. Run the application:
   ```bash
   dotnet run
   ```

2. Navigate to:
   - `http://localhost:5000/` - Public access
   - `http://localhost:5000/user` - Requires Windows authentication

## Code Structure

The application is configured with:
- `NegotiateDefaults.AuthenticationScheme` as the default authentication scheme
- Authorization middleware to protect specific endpoints
- Automatic user identity extraction from Windows credentials

## Security Notes

- This authentication method only works on Windows environments
- Users must be logged into a Windows domain or local machine
- The application will automatically use the current Windows user's credentials