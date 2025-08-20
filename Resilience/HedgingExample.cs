#!/usr/bin/env dotnet
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using System.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("primary-endpoint", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
});

builder.Services.AddHttpClient("backup-endpoint", client =>
{
    client.BaseAddress = new Uri("https://postman-echo.com/");
});

builder.Services.AddHttpClient<GeographicHedgingService>("geo-hedging")
.AddHedgingHandler(options =>
{
    options.Endpoint = "primary-endpoint";
    options.BackupEndpoint = "backup-endpoint";
    
    options.HedgingDelay = TimeSpan.FromSeconds(2);
    options.MaxHedgedAttempts = 2;
    
    options.OnHedging = args =>
    {
        Console.WriteLine($"🔀 Hedging triggered after {args.HedgingDelay} - starting backup request");
        return ValueTask.CompletedTask;
    };
});

builder.Services.AddHttpClient<LoadBalancingService>("load-balancing")
.AddHedgingHandler(options =>
{
    options.Endpoint = "primary-endpoint";
    options.BackupEndpoint = "backup-endpoint";
    
    options.HedgingDelay = TimeSpan.FromMilliseconds(500);
    options.MaxHedgedAttempts = 3;
    
    options.OnHedging = args =>
    {
        Console.WriteLine($"⚖️  Load balancing - hedging attempt {args.AttemptNumber}");
        return ValueTask.CompletedTask;
    };
});

builder.Services.AddHttpClient<PerformanceOptimizedService>("perf-optimized")
.AddHedgingHandler(options =>
{
    options.Endpoint = "primary-endpoint";
    options.BackupEndpoint = "backup-endpoint";
    
    options.HedgingDelay = TimeSpan.FromMilliseconds(100);
    options.MaxHedgedAttempts = 5;
    
    options.OnHedging = args =>
    {
        Console.WriteLine($"🚀 Performance hedging - parallel request #{args.AttemptNumber}");
        return ValueTask.CompletedTask;
    };
});

var host = builder.Build();

var geoService = host.Services.GetRequiredService<GeographicHedgingService>();
var loadBalancingService = host.Services.GetRequiredService<LoadBalancingService>();
var perfService = host.Services.GetRequiredService<PerformanceOptimizedService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

async Task TestHedging<T>(T service, string name, Func<T, Task> testMethod)
{
    Console.WriteLine($"\n=== Testing {name} Hedging ===");
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        await testMethod(service);
        stopwatch.Stop();
        Console.WriteLine($"✅ Completed in {stopwatch.ElapsedMilliseconds}ms");
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"❌ Failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
    }
    
    await Task.Delay(1000);
}

await TestHedging(geoService, "Geographic Failover", async service =>
{
    await service.GetDataWithGeoFailoverAsync();
});

await TestHedging(geoService, "Geographic with Slow Primary", async service =>
{
    await service.GetDataWithSlowPrimaryAsync();
});

await TestHedging(loadBalancingService, "Load Balancing", async service =>
{
    await service.GetDataWithLoadBalancingAsync();
});

await TestHedging(perfService, "Performance Optimized", async service =>
{
    await service.GetFastestResponseAsync();
});

Console.WriteLine("\n=== Demonstrating Hedging Benefits ===");
Console.WriteLine("Running multiple requests to show response time improvements...");

for (int i = 1; i <= 5; i++)
{
    var sw = Stopwatch.StartNew();
    try
    {
        await perfService.GetFastestResponseAsync();
        sw.Stop();
        Console.WriteLine($"Request {i}: ✅ {sw.ElapsedMilliseconds}ms");
    }
    catch
    {
        sw.Stop();
        Console.WriteLine($"Request {i}: ❌ {sw.ElapsedMilliseconds}ms");
    }
}

public class GeographicHedgingService(HttpClient httpClient, ILogger<GeographicHedgingService> logger)
{
    public async Task<string> GetDataWithGeoFailoverAsync()
    {
        logger.LogInformation("Testing geographic failover with normal response time");
        
        var response = await httpClient.GetAsync("get?region=primary");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<string> GetDataWithSlowPrimaryAsync()
    {
        logger.LogInformation("Testing with intentionally slow primary endpoint");
        
        var response = await httpClient.GetAsync("delay/3");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}

public class LoadBalancingService(HttpClient httpClient, ILogger<LoadBalancingService> logger)
{
    public async Task<string> GetDataWithLoadBalancingAsync()
    {
        logger.LogInformation("Testing load balancing across multiple endpoints");
        
        var response = await httpClient.GetAsync("get?balanced=true");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}

public class PerformanceOptimizedService(HttpClient httpClient, ILogger<PerformanceOptimizedService> logger)
{
    private readonly Random _random = new();
    
    public async Task<string> GetFastestResponseAsync()
    {
        logger.LogInformation("Getting fastest response using aggressive hedging");
        
        var delay = _random.Next(1, 4);
        var response = await httpClient.GetAsync($"delay/{delay}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}