#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview*

var builder = WebApplicationn.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
