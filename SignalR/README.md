# SignalR Examples

This directory contains comprehensive examples demonstrating how to implement ASP.NET Core SignalR for real-time communication between clients and servers.

## Overview

SignalR is a library for ASP.NET Core that simplifies adding real-time web functionality to applications. These examples show how to:
- Set up SignalR servers with different configurations
- Create SignalR clients with various features
- Implement real-time messaging and broadcasting
- Use different transport protocols and backplanes
- Add authentication, authorization, and filtering
- Handle multiple hubs and advanced scenarios

## Examples

### Basic SignalR Implementation

#### `SignalRServerBasic.cs`
- **Purpose**: Demonstrates the most basic SignalR server setup
- **Features**: 
  - Basic hub implementation with connection lifecycle events
  - Simple message broadcasting to all connected clients
  - Connection and disconnection logging
  - Basic message handling with `SayHello` method
- **Use Case**: Getting started with SignalR or simple real-time messaging

#### `SignalRClientBasic.cs`
- **Purpose**: Basic SignalR client implementation
- **Features**:
  - Simple hub connection setup
  - Message receiving with `On<T>` method
  - Basic message sending with `InvokeAsync`
  - Manual connection management
- **Use Case**: Basic client applications that need to connect to SignalR servers

#### `SignalRClientBasic2.cs`
- **Purpose**: Enhanced SignalR client with advanced features
- **Features**:
  - Automatic reconnection with `WithAutomaticReconnect()`
  - MessagePack protocol for better performance
  - Comprehensive logging configuration
  - Debug-level logging for development
- **Use Case**: Production-ready clients requiring reliability and performance

### Advanced SignalR Features

#### `SignalRServerWithAuthentication.cs`
- **Purpose**: Demonstrates SignalR with JWT authentication
- **Features**:
  - JWT Bearer token authentication
  - Authorization middleware integration
  - `[Authorize]` attribute on hubs
  - Secure real-time communication
- **Use Case**: Applications requiring authenticated real-time communication

#### `SignalRServerStronglyTyped.cs`
- **Purpose**: Shows strongly-typed SignalR hub implementation
- **Features**:
  - Interface-based hub contracts
  - Type-safe client communication
  - Compile-time method validation
  - Better IntelliSense support
- **Use Case**: When you want compile-time safety and better developer experience

#### `SignalRServerWithHubFilter.cs`
- **Purpose**: Demonstrates hub filtering for cross-cutting concerns
- **Features**:
  - Global hub filters
  - Connection and disconnection interception
  - Rate limiting capabilities
  - Custom filter implementation
- **Use Case**: Adding middleware-like functionality to SignalR hubs

#### `SignalRServerWithMinApi.cs`
- **Purpose**: Shows SignalR integration with Minimal APIs
- **Features**:
  - Minimal API endpoints alongside SignalR
  - Hub context usage in minimal APIs
  - Combined approach for modern applications
- **Use Case**: Modern ASP.NET Core applications using minimal APIs

#### `SignalRServerWithMultipleHubs.cs`
- **Purpose**: Demonstrates multiple hub management
- **Features**:
  - Multiple hub classes (`BasicHub`, `AnotherHub`)
  - Separate hub endpoints (`/basic`, `/anotherhub`)
  - Independent hub functionality
  - Scalable hub architecture
- **Use Case**: Applications requiring different types of real-time communication

### Performance and Scalability

#### `SignalRServerWithMesagePack.cs`
- **Purpose**: Shows MessagePack protocol integration for better performance
- **Features**:
  - MessagePack serialization protocol
  - Reduced message size and improved performance
  - Binary message format
  - Better network efficiency
- **Use Case**: High-performance applications with large message volumes

#### `SignalRServerBackplaneRedis.cs`
- **Purpose**: Demonstrates Redis backplane for SignalR scaling
- **Features**:
  - Redis-based message backplane
  - Horizontal scaling support
  - Load balancer compatibility
  - Distributed SignalR applications
- **Use Case**: Applications requiring horizontal scaling across multiple servers

#### `SignalRServerBackplaneAzure.cs`
- **Purpose**: Shows Azure SignalR Service integration
- **Features**:
  - Azure SignalR Service backplane
  - Managed SignalR infrastructure
  - Azure cloud integration
  - Enterprise-grade scaling
- **Use Case**: Cloud-based applications using Azure services

### Monitoring and Health

#### `SignalRServerHealthCheck.cs`
- **Purpose**: Integrates health checks with SignalR
- **Features**:
  - Health check endpoint registration
  - SignalR service health monitoring
  - Connection status tracking
  - Production monitoring capabilities
- **Use Case**: Production applications requiring health monitoring

## Getting Started

### Prerequisites
- .NET 10.0 or later
- ASP.NET Core
- For backplane examples: Redis or Azure SignalR Service

### Basic Setup
1. Add the required packages:
   ```bash
   dotnet add package Microsoft.AspNetCore.SignalR
   dotnet add package Microsoft.AspNetCore.SignalR.Client
   ```

2. Register SignalR services:
   ```csharp
   builder.Services.AddSignalR();
   ```

3. Map hub endpoints:
   ```csharp
   app.MapHub<YourHub>("/signalr");
   ```

### Running the Examples
Each example can be run independently:

```bash
# Server examples
dotnet SignalRServerBasic.cs

# Client examples
dotnet SignalRClientBasic.cs
```

## Key Concepts

### Hubs
- **Hubs** are the main abstraction in SignalR
- They handle client connections and provide methods for client-server communication
- Clients can call methods on the server, and servers can call methods on clients

### Connection Lifecycle
- **OnConnectedAsync()**: Called when a client connects
- **OnDisconnectedAsync()**: Called when a client disconnects
- **ConnectionId**: Unique identifier for each client connection

### Client Communication
- **Clients.All**: Send to all connected clients
- **Clients.Caller**: Send to the calling client
- **Clients.Others**: Send to all clients except the caller
- **Clients.Group()**: Send to clients in a specific group

### Transport Protocols
- **WebSockets**: Best performance, full-duplex communication
- **Server-Sent Events**: Server-to-client streaming
- **Long Polling**: Fallback for older browsers

## Best Practices

1. **Use Automatic Reconnection**: Implement `WithAutomaticReconnect()` for production clients
2. **Handle Connection States**: Monitor connection state changes
3. **Implement Error Handling**: Handle connection failures gracefully
4. **Use MessagePack**: Consider MessagePack for better performance
5. **Implement Backplanes**: Use Redis or Azure SignalR Service for scaling
6. **Add Authentication**: Secure your real-time communication
7. **Monitor Health**: Implement health checks for production applications

## Common Use Cases

- **Real-time Chat Applications**: Instant messaging, group chats
- **Live Dashboards**: Real-time data updates, monitoring
- **Collaborative Tools**: Document editing, whiteboarding
- **Gaming**: Real-time game state updates
- **Notifications**: Push notifications, alerts
- **Live Streaming**: Real-time content delivery

## Resources

- [ASP.NET Core SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [SignalR Client Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/dotnet-client)
- [SignalR JavaScript Client](https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client)
- [Azure SignalR Service](https://azure.microsoft.com/en-us/services/signalr-service/)
- [Redis Documentation](https://redis.io/documentation)

## Contributing

Feel free to add more examples or improve existing ones. Each example should demonstrate a specific SignalR feature or use case clearly. Consider adding examples for:
- SignalR with Blazor
- SignalR with Angular/React
- Advanced filtering scenarios
- Custom transport implementations
- Performance optimization techniques
