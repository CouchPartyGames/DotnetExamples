# Certificate Authentication Example

This example demonstrates mutual TLS (mTLS) client certificate authentication in ASP.NET Core using Kestrel server.

## Overview

The project consists of three main components:

1. **Certificate Generation Script** (`create-certs.sh`) - Creates server and client certificates using mkcert
2. **ASP.NET Core Server** (`CertificateAuthentication.cs`) - Configures certificate authentication with Kestrel
3. **HTTP Client** (`ClientRequestWithCertificate.cs`) - Makes authenticated requests using client certificates

## Setup

### Prerequisites
- .NET SDK
- mkcert (for certificate generation)

### Generate Certificates

Run the certificate generation script:

```bash
./create-certs.sh
```

This will:
- Install mkcert CA root certificate
- Generate server certificate (`server.certificate.pem`, `server.private.key.pem`)
- Generate client certificate (`client.certificate.pem`, `client.private.key.pem`)
- Display CA root location

## Server Configuration

The server (`CertificateAuthentication.cs`) configures:

- **Kestrel HTTPS**: Listens on port 5001 with TLS client certificate requirement
- **Certificate Authentication**: Validates client certificates and creates claims
- **Authorization**: Protects `/protected` endpoint

### Key Features
- Requires client certificates for all HTTPS connections
- Validates certificate use and validity period
- Creates claims from certificate subject
- Provides authentication event handlers for logging

## Client Usage

The client (`ClientRequestWithCertificate.cs`):

- Loads client certificate from PEM files
- Configures HttpClient with client certificate
- Makes authenticated request to `/protected` endpoint
- Accepts any server certificate (for development)

### Endpoints

- `GET /` - Public endpoint returning "Hello World!"
- `GET /protected` - Protected endpoint requiring certificate authentication

## Running the Example

1. Generate certificates: `./create-certs.sh`
2. Start server: `dotnet CertificateAuthentication.cs`
3. Make client request: `dotnet ClientRequestWithCertificate.cs`

## Security Notes

- Client certificate validation happens at TLS level before reaching ASP.NET Core
- Server accepts any valid client certificate (`CertificateTypes.All`)
- Certificate revocation checking is disabled for development
- Client accepts any server certificate (development only)

## Authentication Flow

1. Client initiates TLS handshake with server
2. Server requests client certificate
3. Client presents certificate during TLS negotiation
4. Kestrel validates certificate and passes to ASP.NET Core
5. Certificate authentication middleware creates claims principal
6. Authorization middleware checks access to protected resources
