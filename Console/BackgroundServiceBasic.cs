#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Worker
#:package Microsoft.Extensions.Hosting@9.0.6

using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MyBackgroundService>();
var app = builder.Build();
await app.RunAsync();


public sealed class MyBackgroundService : BackgroundService {

	protected override async Task ExecuteAsync(CancellationToken stoppingToken = default) {
		using PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
		while (await _timer.WaitForNextTickAsync(stoppingToken))
		{
			Console.WriteLine("Doing Working");
		}
	}


	public override Task StartAsync(CancellationToken stoppingToken)
	{
		Console.WriteLine("Starting...");
		return Task.CompletedTask;
	}

	public override Task StopAsync(CancellationToken stoppingToken)
	{
		Console.WriteLine("Stopping...");
		return Task.CompletedTask;
	}
}
