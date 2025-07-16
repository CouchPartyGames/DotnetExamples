#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@1.2.0

using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
// Inject SignalR
builder.Services.AddSignalR();
var app = builder.Build();

// Setup Hub to handle messages
app.MapHub<BasicHub>("/signalr");
app.MapGet("/", (IHubContext hubContext) =>
{
    hubContext.SayHello();
});
app.Run();

public sealed class BasicHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("OnConnectedAsync");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Console.WriteLine("OnDisconnectedAsync");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task SayHello() {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
