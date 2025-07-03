#!/usr/bin/env -S dotnet run
#:package Microsoft.Extensions.Hosting@9.0.*
#:package NetCord@1.0.0-alpha.391
#:package NetCord.Hosting@1.0.0-alpha.391
#:package NetCord.Services@1.0.0-alpha.391


// Intents allow you to subscribe Discord events (Gateway Events),
// like MessageCreate and GuildUserAdd using WebSockets (GatewayClient).

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;


var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.GuildMessages
                          | GatewayIntents.DirectMessages
                          | GatewayIntents.MessageContent
                          | GatewayIntents.DirectMessageReactions
                          | GatewayIntents.GuildMessageReactions;
        options.Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
    })
    .AddGatewayHandlers(typeof(Program).Assembly);

var host = builder.Build()
    .UseGatewayHandlers();

await host.RunAsync();

public class MessageCreateHandler(ILogger<MessageCreateHandler> logger) : IMessageCreateGatewayHandler
{
    public ValueTask HandleAsync(Message message)
    {
        logger.LogInformation("{}", message.Content);
        return default;
    }
}

public class MessageReactionAddHandler(RestClient client) : IMessageReactionAddGatewayHandler
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        await client.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
    }
}