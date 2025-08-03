#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview*

using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication()
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
			ValidIssuer = builder.Configuration["Supabase:ValidIssuer"],
			ValidAudience = builder.Configuration["Supabase:ValidAudience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Supabase:JwtSecret"])),
			
			
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/user", (ClaimsPrincipal principal) =>
{
	var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
	return Results.Ok(claims);
}).RequireAuthorization();

app.Run();
