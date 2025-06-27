#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@1.2.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview*


using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);
// Inject SignalR
builder.Services.AddSignalR();
builder.Services.AddAuthentication()
    .AddJwtBearer();
builder.Services.AddAuthorization();
var app = builder.Build();

// Add authentication/authorization middleware
// Note: Authentication middleware must come BEFORE authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<BasicHub>("/signalr");
app.Run();


[Authorize]
public sealed class BasicHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}