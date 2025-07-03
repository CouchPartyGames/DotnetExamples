#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Worker
#:package Microsoft.Extensions.Hosting@9.0.6

using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MyBackgroundService>();
var app = builder.Build();
await app.RunAsync();


public sealed class MyBackgroundService(ILogger<MyBackgroundService> logger) : BackgroundService {

	protected override async Task ExecuteAsync(CancellationToken stoppingToken = default) {
		logger.LogInformation("Do Work");
		using PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
		while (await _timer.WaitForNextTickAsync(stoppingToken))
		{
			logger.LogInformation("Do Work");
		}
		return Task.CompletedTask;
	}


	public override Task StartAsync(CancellationToken stoppingToken) {
		logger.LogInformation("Started");
		return Task.CompletedTask;
	}

	public override Task StopAsync(CancellationToken stoppingToken) {
		logger.LogInformation("Stopped");
		return Task.CompletedTask;
	}
}
