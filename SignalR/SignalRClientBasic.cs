#!/usr/bin/env -S dotnet run
#:package Microsoft.AspNetCore.SignalR.Client@9.0.*

using Microsoft.AspNetCore.SignalR.Client;

var userName = "admin";
var serverAddress = "http://localhost:5000/signalr/";

var hubConnection = new HubConnectionBuilder()
    .WithUrl(serverAddress)
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
