#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview*

using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer(opts =>
    {
        opts.RequireHttpsMetadata = false;
        opts.MetadataAddress = "todo";
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            //ValidIssuer = "todo";
        };
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
