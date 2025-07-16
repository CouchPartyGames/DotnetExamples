#!/usr/bin/env dotnet 
#:package Microsoft.Extensions.Hosting@9.0.*
#:package NetCord@1.0.0-alpha.391
#:package NetCord.Hosting@1.0.0-alpha.391
#:package NetCord.Hosting.Services@1.0.0-alpha.391
#:property UserSecretsId dotnet-examples

// Discord Interactions using HTTP
// Interactions like user executing a slash command

using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddDiscordGateway(opts =>
    {
        opts.Token = builder.Configuration["Discord:Token"];
    })
    .AddApplicationCommands();

var host = builder.Build();
host.AddModules(typeof(Program).Assembly);
host.UseGatewayHandlers();		// Required for Console Interactions
await host.RunAsync();


public sealed class PingHandler : ApplicationCommandModule<ApplicationCommandContext> {
    
    [SlashCommand("ping", "Ping the server")]
    public static string Ping() => "Pong!";

    [UserCommand("ID")]
    public static string Id(User user) => user.Id.ToString();

    [MessageCommand("Timestamp")]
    public static string Timestamp(RestMessage message) => message.CreatedAt.ToString();
}
