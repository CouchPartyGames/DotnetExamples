#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.InMemory@9.0.*
#:package Microsoft.EntityFrameworkCore.Tools@9.0.*
#:package Microsoft.AspNetCore.Identity@2.3.*
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@9.0.*
#:package Microsoft.AspNetCore.OpenApi@9.0.*
#:package Scalar.AspNetCore@2.4.22
#:package MailKit@4.13.0
#:property PublishAot=false     
// Must disable AOT

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication()
    .AddCookie("Identity.Bearer");
builder.Services.AddAuthorization();

// Add Database Dependency
builder.Services.AddDbContext<AppDbContext>(
    opts => opts.UseInMemoryDatabase("MyTestDatabase"));

// Add Identity Core
builder.Services.AddIdentityCore<IdentityUser>(opts =>
    {
        opts.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();
//builder.Services.AddIdentityApiEndpoints<IdentityUser>()
//    .AddEntityFrameworkStores<AppDbContext>();
//.AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(opts => {
    opts.AccessDeniedPath = "/account/denied";
});

builder.Services.AddTransient<IEmailSender, EmailSenderService>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Authentication Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();
app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"))
    .Produces(302);

app.Run();

public sealed class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) {}
}

public sealed class EmailSenderService : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Subject = ".NET Core Identity Sample with Email Sender";
        message.To.Add(new MailboxAddress ("Some User", "user@domain.com"));
        message.From.Add(new MailboxAddress ("Other User", "other@domain.com"));
        message.Body = new TextPart("plain")
        {
            Text = "hi"
        };
        
        using var client = new SmtpClient();
        await client.ConnectAsync("localhost", 587, false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}