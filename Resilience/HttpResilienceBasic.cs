#!/usr/bin/env dotnet
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<WeatherService>("weather-client", client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.DefaultRequestHeaders.Add("User-Agent", "DotnetExample/1.0");
})
.AddStandardResilienceHandler();

builder.Services.AddHttpClient<SlowApiService>("slow-api", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.MinimumThroughput = 5;
});

var host = builder.Build();

var weatherService = host.Services.GetRequiredService<WeatherService>();
var slowApiService = host.Services.GetRequiredService<SlowApiService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Testing standard resilience handler...");

try
{
    await weatherService.GetWeatherAsync("London");
    logger.LogInformation("Weather API call succeeded");
}
catch (Exception ex)
{
    logger.LogError(ex, "Weather API call failed");
}

try
{
    await slowApiService.TestDelayAsync(5);
    logger.LogInformation("Slow API call succeeded");
}
catch (Exception ex)
{
    logger.LogError(ex, "Slow API call failed");
}

public sealed class WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
{
    public async Task<string> GetWeatherAsync(string city)
    {
        logger.LogInformation("Calling weather API for {City}", city);
        
        var response = await httpClient.GetAsync($"data/2.5/weather?q={city}&appid=demo");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}

public sealed class SlowApiService(HttpClient httpClient, ILogger<SlowApiService> logger)
{
    public async Task<string> TestDelayAsync(int seconds)
    {
        logger.LogInformation("Testing delay endpoint with {Seconds} seconds", seconds);
        
        var response = await httpClient.GetAsync($"delay/{seconds}");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}