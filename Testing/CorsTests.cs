#!/usr/bin/env dotnet
#:package TUnit@0.25.21
#:package Microsoft.AspNetCore.TestHost@9.0.0
#:package Microsoft.AspNetCore.Mvc.Testing@9.0.0

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Testing;

public class CorsTests
{
    [Test]
    public async Task CorsPolicy_AllowsSpecificOrigin_ReturnsAccessControlHeaders()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("TestPolicy", policy =>
                        {
                            policy.WithOrigins("https://example.com")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
                    });
                });
                
                builder.Configure(app =>
                {
                    app.UseCors("TestPolicy");
                    app.UseRouting();
                    app.MapGet("/api/test", () => "Hello World");
                });
            });

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("Origin", "https://example.com");

        var response = await client.GetAsync("/api/test");

        await Assert.That(response.Headers.Contains("Access-Control-Allow-Origin")).IsTrue();
        await Assert.That(response.Headers.GetValues("Access-Control-Allow-Origin").First())
            .IsEqualTo("https://example.com");
    }

    [Test]
    public async Task CorsPolicy_BlocksUnauthorizedOrigin_DoesNotReturnAccessControlHeaders()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("TestPolicy", policy =>
                        {
                            policy.WithOrigins("https://example.com")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
                    });
                });
                
                builder.Configure(app =>
                {
                    app.UseCors("TestPolicy");
                    app.UseRouting();
                    app.MapGet("/api/test", () => "Hello World");
                });
            });

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("Origin", "https://malicious-site.com");

        var response = await client.GetAsync("/api/test");

        await Assert.That(response.Headers.Contains("Access-Control-Allow-Origin")).IsFalse();
    }

    [Test]
    public async Task CorsPreflightRequest_AllowedOriginAndMethod_ReturnsCorrectHeaders()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("TestPolicy", policy =>
                        {
                            policy.WithOrigins("https://example.com")
                                  .WithMethods("GET", "POST", "PUT")
                                  .AllowAnyHeader();
                        });
                    });
                });
                
                builder.Configure(app =>
                {
                    app.UseCors("TestPolicy");
                    app.UseRouting();
                    app.MapPost("/api/data", () => "Data received");
                });
            });

        var client = application.CreateClient();
        
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/data");
        request.Headers.Add("Origin", "https://example.com");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "Content-Type");

        var response = await request.SendAsync(client);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await Assert.That(response.Headers.Contains("Access-Control-Allow-Origin")).IsTrue();
        await Assert.That(response.Headers.Contains("Access-Control-Allow-Methods")).IsTrue();
        await Assert.That(response.Headers.GetValues("Access-Control-Allow-Methods"))
            .Contains("POST");
    }

    [Test]
    [Arguments("GET")]
    [Arguments("POST")]
    [Arguments("PUT")]
    public async Task CorsPolicy_AllowedMethods_ReturnsSuccess(string httpMethod)
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("TestPolicy", policy =>
                        {
                            policy.AllowAnyOrigin()
                                  .WithMethods("GET", "POST", "PUT")
                                  .AllowAnyHeader();
                        });
                    });
                });
                
                builder.Configure(app =>
                {
                    app.UseCors("TestPolicy");
                    app.UseRouting();
                    app.MapMethods("/api/test", new[] { "GET", "POST", "PUT" }, () => "Success");
                });
            });

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("Origin", "https://example.com");

        var request = new HttpRequestMessage(new HttpMethod(httpMethod), "/api/test");
        var response = await request.SendAsync(client);

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        await Assert.That(response.Headers.Contains("Access-Control-Allow-Origin")).IsTrue();
    }

    [Test]
    public async Task CorsPolicy_WithCredentials_ReturnsAllowCredentialsHeader()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("TestPolicy", policy =>
                        {
                            policy.WithOrigins("https://example.com")
                                  .AllowCredentials()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
                    });
                });
                
                builder.Configure(app =>
                {
                    app.UseCors("TestPolicy");
                    app.UseRouting();
                    app.MapGet("/api/secure", () => "Secure data");
                });
            });

        var client = application.CreateClient();
        client.DefaultRequestHeaders.Add("Origin", "https://example.com");

        var response = await client.GetAsync("/api/secure");

        await Assert.That(response.Headers.Contains("Access-Control-Allow-Credentials")).IsTrue();
        await Assert.That(response.Headers.GetValues("Access-Control-Allow-Credentials").First())
            .IsEqualTo("true");
    }

    [Test]
    public async Task CorsPolicy_CustomHeaders_AllowsSpecifiedHeaders()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("TestPolicy", policy =>
                        {
                            policy.AllowAnyOrigin()
                                  .WithHeaders("X-Custom-Header", "Content-Type")
                                  .AllowAnyMethod();
                        });
                    });
                });
                
                builder.Configure(app =>
                {
                    app.UseCors("TestPolicy");
                    app.UseRouting();
                    app.MapPost("/api/custom", () => "Custom header received");
                });
            });

        var client = application.CreateClient();
        
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/custom");
        request.Headers.Add("Origin", "https://example.com");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "X-Custom-Header,Content-Type");

        var response = await request.SendAsync(client);

        await Assert.That(response.Headers.Contains("Access-Control-Allow-Headers")).IsTrue();
        var allowedHeaders = response.Headers.GetValues("Access-Control-Allow-Headers").First();
        await Assert.That(allowedHeaders).Contains("X-Custom-Header");
        await Assert.That(allowedHeaders).Contains("Content-Type");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        app.MapGet("/", () => "Hello World!");
        app.Run();
    }
}