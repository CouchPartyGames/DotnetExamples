#!/usr/bin/env -S dotnet run
#:package NetCord@1.0.0-alpha.391
#:package NetCord.Hosting.AspNetCore@1.0.0-alpha.391
#:package NetCord.Hosting.Services@1.0.0-alpha.391

// Discord Interactions using HTTP
// Interactions like user executing a slash command

using NetCord.Hosting.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDiscordRest(opt =>
    {
        opt.Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        opt.PublicKey = Environment.GetEnvironmentVariable("DISCORD_API_KEY");
    })
    .AddHttpApplicationCommands(opts => {
        // Add Localization Support (Requires the folder to exist)
        opts.LocalizationsProvider = new JsonLocalizationsProvider();
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.AddSlashCommand("cheese", "Get cheesed", () => "Hi, i'm a cheese");

app.UseHttpInteractions("/v1/interactions");
await app.RunAsync();
