#!/usr/bin/env dotnet
#:package Microsoft.Extensions.Hosting.Abstractions@10.*-*


// 4. Timeout Strategies
//
// File: Resilience/TimeoutStrategies.cs
// - Quick Service: 2-second timeout for fast operations
// - Patient Service: 10-second timeout for slower operations
// - Multi-Level: Combined total timeout (30s) and per-attempt timeout (5s) with retries
// - File Download: Long timeout (5 minutes) for large transfers

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<ReliableService>("reliable-service", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("reliable-circuit-breaker", static builder =>
{
    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        MinimumThroughput = 3,
        SamplingDuration = TimeSpan.FromSeconds(10),
        BreakDuration = TimeSpan.FromSeconds(5),
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(response => (int)response.StatusCode >= 500),
        OnOpened = args =>
        {
            Console.WriteLine($"🔴 Circuit breaker OPENED at {DateTime.Now:HH:mm:ss}");
            return ValueTask.CompletedTask;
        },
        OnClosed = args =>
        {
            Console.WriteLine($"🟢 Circuit breaker CLOSED at {DateTime.Now:HH:mm:ss}");
            return ValueTask.CompletedTask;
        },
        OnHalfOpened = args =>
        {
            Console.WriteLine($"🟡 Circuit breaker HALF-OPENED at {DateTime.Now:HH:mm:ss}");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<SensitiveService>("sensitive-service", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("sensitive-circuit-breaker", static builder =>
{
    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        FailureRatio = 0.2,
        MinimumThroughput = 2,
        SamplingDuration = TimeSpan.FromSeconds(5),
        BreakDuration = TimeSpan.FromSeconds(15),
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<Exception>()
            .HandleResult(response => !response.IsSuccessStatusCode),
        OnOpened = args =>
        {
            Console.WriteLine($"⚠️  Sensitive circuit breaker OPENED - protecting service");
            return ValueTask.CompletedTask;
        },
        OnClosed = args =>
        {
            Console.WriteLine($"✅ Sensitive circuit breaker CLOSED - service recovered");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<FastFailService>("fast-fail-service", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("fast-fail", static builder =>
{
    builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        FailureRatio = 1.0,
        MinimumThroughput = 1,
        SamplingDuration = TimeSpan.FromSeconds(1),
        BreakDuration = TimeSpan.FromSeconds(30),
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<Exception>(),
        OnOpened = args =>
        {
            Console.WriteLine($"🚨 Fast-fail circuit breaker OPENED - immediate protection");
            return ValueTask.CompletedTask;
        }
    });
});

var host = builder.Build();

var reliableService = host.Services.GetRequiredService<ReliableService>();
var sensitiveService = host.Services.GetRequiredService<SensitiveService>();
var fastFailService = host.Services.GetRequiredService<FastFailService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

async Task TestCircuitBreaker<T>(T service, string name, Func<T, Task> testMethod)
{
    Console.WriteLine($"\n=== Testing {name} Circuit Breaker ===");
    
    for (int i = 1; i <= 10; i++)
    {
        try
        {
            Console.Write($"Request {i}: ");
            await testMethod(service);
            Console.WriteLine("✅ SUCCESS");
            await Task.Delay(500);
        }
        catch (BrokenCircuitException)
        {
            Console.WriteLine("🔴 BLOCKED - Circuit breaker is open");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ FAILED - {ex.Message}");
        }
        
        await Task.Delay(1000);
    }
}

await TestCircuitBreaker(reliableService, "Reliable", async service =>
{
    await service.TestFailingEndpoint();
});

await TestCircuitBreaker(sensitiveService, "Sensitive", async service =>
{
    await service.TestUnstableEndpoint();
});

await TestCircuitBreaker(fastFailService, "Fast-Fail", async service =>
{
    await service.TestAlwaysFailEndpoint();
});

public class ReliableService(HttpClient httpClient, ILogger<ReliableService> logger)
{
    public async Task TestFailingEndpoint()
    {
        var response = await httpClient.GetAsync("status/500");
        response.EnsureSuccessStatusCode();
    }
}

public class SensitiveService(HttpClient httpClient, ILogger<SensitiveService> logger)
{
    private readonly Random _random = new();
    
    public async Task TestUnstableEndpoint()
    {
        var statusCode = _random.NextDouble() < 0.7 ? 200 : 503;
        var response = await httpClient.GetAsync($"status/{statusCode}");
        response.EnsureSuccessStatusCode();
    }
}

public class FastFailService(HttpClient httpClient, ILogger<FastFailService> logger)
{
    public async Task TestAlwaysFailEndpoint()
    {
        throw new HttpRequestException("Simulated network failure");
    }
}
