#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Simple Example of running Kestrel on 2 ports (http and https2)
// openssl pkcs12 -export -out certificate.pfx -inkey privateKey.key -in certificate.crt -certfile more.crt
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("<path to .pfx file>",
            "<certificate password>");
    });
});

var app = builder.Build();
app.UseHttpsRedirection();
app.MapGet("/", () => "Hello World!");
app.Run();