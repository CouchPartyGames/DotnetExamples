#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@1.2.0

using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
// Inject SignalR
builder.Services.AddSignalR(opts =>
{
        // Global Filter
    opts.AddFilter<RateLimitFilter>();
});
var app = builder.Build();

// Setup Hub to handle messages
app.MapHub<BasicHub>("/signalr");
app.MapGet("/", (IHubContext hubContext) =>
{
    hubContext.SayHello();
});
app.Run();


public sealed class RateLimitFilter : IHubFilter
{
    
    // Optional method
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        return next(context);
    }

    // Optional method
    public Task OnDisconnectedAsync(
        HubLifetimeContext context, Exception exception, Func<HubLifetimeContext, Exception, Task> next)
    {
        return next(context, exception);
    }
}