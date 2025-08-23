#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@1.2.0
#:package Microsoft.AspNetCore.SignalR.StackExchangeRedis@10.*-* 

using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Inject SignalR using Redis Backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis("<your_Redis_connection_string>");

var app = builder.Build();

// Setup Hub to handle messages
// SignalR Hubs enables connected clients to call methods on the serve
app.MapHub<BasicHub>("/signalr");
app.Run();

public sealed class BasicHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"OnConnectedAsync: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Console.WriteLine($"OnDisconnectedAsync: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task SayHello(string user, string message) {
        Console.WriteLine($"Message From {user}: {message}");
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}