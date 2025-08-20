#!/usr/bin/env dotnet
#:package Microsoft.Extensions.Hosting.Abstractions@10.*-*

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using System.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<QuickResponseService>("quick-service", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("quick-timeout", static builder =>
{
    builder.AddTimeout(new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromSeconds(2),
        OnTimeout = args =>
        {
            Console.WriteLine($"⏰ Quick service timed out after {args.Timeout}");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<PatientService>("patient-service", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("patient-timeout", static builder =>
{
    builder.AddTimeout(new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromSeconds(10),
        OnTimeout = args =>
        {
            Console.WriteLine($"⏱️  Patient service timed out after {args.Timeout}");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<MultiLevelTimeoutService>("multi-level", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddResilienceHandler("multi-level-timeout", static builder =>
{
    builder.AddTimeout(new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromSeconds(30),
        Name = "TotalTimeout",
        OnTimeout = args =>
        {
            Console.WriteLine($"🚨 Total request timeout after {args.Timeout}");
            return ValueTask.CompletedTask;
        }
    });
    
    builder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Linear
    });
    
    builder.AddTimeout(new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromSeconds(5),
        Name = "AttemptTimeout",
        OnTimeout = args =>
        {
            Console.WriteLine($"⚡ Individual attempt timeout after {args.Timeout}");
            return ValueTask.CompletedTask;
        }
    });
});

builder.Services.AddHttpClient<FileDownloadService>("file-download", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
    client.Timeout = Timeout.InfiniteTimeSpan;
})
.AddResilienceHandler("download-timeout", static builder =>
{
    builder.AddTimeout(new HttpTimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromMinutes(5),
        OnTimeout = args =>
        {
            Console.WriteLine($"📁 File download timed out after {args.Timeout}");
            return ValueTask.CompletedTask;
        }
    });
});

var host = builder.Build();

var quickService = host.Services.GetRequiredService<QuickResponseService>();
var patientService = host.Services.GetRequiredService<PatientService>();
var multiLevelService = host.Services.GetRequiredService<MultiLevelTimeoutService>();
var downloadService = host.Services.GetRequiredService<FileDownloadService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

async Task TestWithTiming<T>(T service, string name, Func<T, Task> testMethod)
{
    Console.WriteLine($"\n=== Testing {name} ===");
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        await testMethod(service);
        stopwatch.Stop();
        Console.WriteLine($"✅ Completed in {stopwatch.ElapsedMilliseconds}ms");
    }
    catch (TimeoutRejectedException ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"⏰ Timeout after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"❌ Failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
    }
}

await TestWithTiming(quickService, "Quick Service (2s timeout) with 1s delay", async service =>
{
    await service.TestDelayAsync(1);
});

await TestWithTiming(quickService, "Quick Service (2s timeout) with 3s delay", async service =>
{
    await service.TestDelayAsync(3);
});

await TestWithTiming(patientService, "Patient Service (10s timeout) with 5s delay", async service =>
{
    await service.TestDelayAsync(5);
});

await TestWithTiming(patientService, "Patient Service (10s timeout) with 12s delay", async service =>
{
    await service.TestDelayAsync(12);
});

await TestWithTiming(multiLevelService, "Multi-Level Timeout with 3s delay", async service =>
{
    await service.TestDelayAsync(3);
});

await TestWithTiming(multiLevelService, "Multi-Level Timeout with 7s delay (triggers retry)", async service =>
{
    await service.TestDelayAsync(7);
});

await TestWithTiming(downloadService, "File Download Service with large response", async service =>
{
    await service.DownloadLargeFileAsync();
});

public sealed class QuickResponseService(HttpClient httpClient, ILogger<QuickResponseService> logger)
{
    public async Task<string> TestDelayAsync(int seconds)
    {
        logger.LogInformation("Quick service - requesting {Seconds}s delay", seconds);
        var response = await httpClient.GetAsync($"delay/{seconds}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

public sealed class PatientService(HttpClient httpClient, ILogger<PatientService> logger)
{
    public async Task<string> TestDelayAsync(int seconds)
    {
        logger.LogInformation("Patient service - requesting {Seconds}s delay", seconds);
        var response = await httpClient.GetAsync($"delay/{seconds}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

public sealed class MultiLevelTimeoutService(HttpClient httpClient, ILogger<MultiLevelTimeoutService> logger)
{
    public async Task<string> TestDelayAsync(int seconds)
    {
        logger.LogInformation("Multi-level timeout - requesting {Seconds}s delay", seconds);
        var response = await httpClient.GetAsync($"delay/{seconds}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

public class FileDownloadService(HttpClient httpClient, ILogger<FileDownloadService> logger)
{
    public async Task<byte[]> DownloadLargeFileAsync()
    {
        logger.LogInformation("Downloading large file simulation...");
        var response = await httpClient.GetAsync("bytes/1048576");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}