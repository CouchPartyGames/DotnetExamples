#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@1.2.0

using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();
app.MapHub<MyStronglyTypedHub>("/signalr");
app.Run();

public sealed class MyStronglyTypedHub : Hub<ISuperChat>
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
}

public interface ISuperChat
{
    
};