#!/usr/bin/env dotnet
#:package NetCord@1.0.0-alpha.391
#:package NetCord.Hosting.AspNetCore@1.0.0-alpha.391
#:package NetCord.Hosting.Services@1.0.0-alpha.391
#:property UserSecretsId dotnet-examples

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
        opt.Token = builder.Configuration["Discord:Token"];
        opt.PublicKey = builder.Configuration["Discord:PublicKey"];
    })
    .AddHttpApplicationCommands();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.AddSlashCommand("cheese", "Get cheesed", () => "Hi, i'm a cheese");

app.UseHttpInteractions("/v1/interactions");
await app.RunAsync();
