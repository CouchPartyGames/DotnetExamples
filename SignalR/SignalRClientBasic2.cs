#!/usr/bin/env dotnet
#:package Microsoft.AspNetCore.SignalR.Client@10.*-*
#:package Microsoft.AspNetCore.SignalR.Protocols.MessagePack@10.*-*
#:package Microsoft.Extensions.Logging.Console@10.*-*


using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

var userName = "admin";
var serverAddress = "http://localhost:5000/signalr/";

// Configure Client connection to SignalR Server
// Will attempt auto connect on failure
// Use MessagePack for serialized/better performance message communication
var hubConnection = new HubConnectionBuilder()
    .WithUrl(serverAddress)
    .WithAutomaticReconnect()
    .AddMessagePackProtocol()
    .ConfigureLogging(logging =>
    {
        // Log to the Console
        logging.AddConsole();

        // This will set ALL logging to Debug level
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .Build();

await hubConnection.StartAsync();

hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
{
    var newMessage = $"Received from {user}: {message}";
    Console.WriteLine(newMessage);
});

Thread.Sleep(2000); 
await hubConnection.InvokeAsync("SayHello", userName, "Just wanted to say hello");

Console.WriteLine("Press any key to exit...");
var quit = await Console.In.ReadLineAsync();
await hubConnection.StopAsync();
