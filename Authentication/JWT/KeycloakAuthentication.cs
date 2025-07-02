#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview*

using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/user", (ClaimsPrincipal principal) =>
{
    var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
    return Results.Ok(claims);
}).RequireAuthorization();

app.Run();
