#!/usr/bin/env dotnet
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System.Net;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<ApiService>("retry-client", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("custom-retry", static builder =>
{
    builder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 5,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .Handle<TaskCanceledException>()
            .HandleResult(response => 
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.GatewayTimeout ||
                response.StatusCode == HttpStatusCode.TooManyRequests),
        OnRetry = args =>
        {
            Console.WriteLine($"Retry attempt {args.AttemptNumber} after {args.RetryDelay}");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<ConservativeRetryService>("conservative-retry", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("conservative", static builder =>
{
    builder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 2,
        Delay = TimeSpan.FromMilliseconds(500),
        BackoffType = DelayBackoffType.Linear,
        UseJitter = false,
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(response => response.StatusCode >= HttpStatusCode.InternalServerError),
        OnRetry = args =>
        {
            Console.WriteLine($"Conservative retry {args.AttemptNumber}");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<AggressiveRetryService>("aggressive-retry", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("aggressive", static builder =>
{
    builder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 10,
        Delay = TimeSpan.FromMilliseconds(100),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        MaxDelay = TimeSpan.FromSeconds(30),
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<Exception>()
            .HandleResult(response => !response.IsSuccessStatusCode),
        OnRetry = args =>
        {
            Console.WriteLine($"Aggressive retry {args.AttemptNumber}, delay: {args.RetryDelay}");
            return ValueTask.CompletedTask;
        }
    });
});

var host = builder.Build();

var apiService = host.Services.GetRequiredService<ApiService>();
var conservativeService = host.Services.GetRequiredService<ConservativeRetryService>();
var aggressiveService = host.Services.GetRequiredService<AggressiveRetryService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Testing custom retry strategies...");

Console.WriteLine("\n=== Testing Custom Retry (Exponential with Jitter) ===");
try
{
    await apiService.TestStatusAsync(500);
}
catch (Exception ex)
{
    Console.WriteLine($"Custom retry failed: {ex.Message}");
}

Console.WriteLine("\n=== Testing Conservative Retry (Linear, Server Errors Only) ===");
try
{
    await conservativeService.TestStatusAsync(503);
}
catch (Exception ex)
{
    Console.WriteLine($"Conservative retry failed: {ex.Message}");
}

Console.WriteLine("\n=== Testing Aggressive Retry (All Failures) ===");
try
{
    await aggressiveService.TestStatusAsync(404);
}
catch (Exception ex)
{
    Console.WriteLine($"Aggressive retry failed: {ex.Message}");
}

public class ApiService(HttpClient httpClient, ILogger<ApiService> logger)
{
    public async Task<string> TestStatusAsync(int statusCode)
    {
        logger.LogInformation("Testing status code: {StatusCode}", statusCode);
        
        var response = await httpClient.GetAsync($"status/{statusCode}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}

public class ConservativeRetryService(HttpClient httpClient, ILogger<ConservativeRetryService> logger)
{
    public async Task<string> TestStatusAsync(int statusCode)
    {
        logger.LogInformation("Conservative retry - Testing status: {StatusCode}", statusCode);
        
        var response = await httpClient.GetAsync($"status/{statusCode}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}

public class AggressiveRetryService(HttpClient httpClient, ILogger<AggressiveRetryService> logger)
{
    public async Task<string> TestStatusAsync(int statusCode)
    {
        logger.LogInformation("Aggressive retry - Testing status: {StatusCode}", statusCode);
        
        var response = await httpClient.GetAsync($"status/{statusCode}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}