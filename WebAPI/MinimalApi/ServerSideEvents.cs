#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false     
// Must disable AOT

using System.Runtime.CompilerServices;
using System.Net.ServerSentEvents;

// .NET 10 added support for SSE (Server Side Events)
var builder = WebApplication.CreateBuilder(args);

// Register Cors Policies
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("everything-open", corsOpts =>
    {
        corsOpts
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();
app.UseCors("everything-open");
app.MapGet("/heartrate-string", (CancellationToken token) =>
{
    async IAsyncEnumerable<string> GetHeartRate(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string heartRate = Random.Shared.Next(60, 100).ToString();
            yield return $"Hear Rate: {heartRate} bpm";
            await Task.Delay(2000, cancellationToken);
        }
    }

    var eventType = "heartrate";
    return TypedResults.ServerSentEvents(GetHeartRate(token), eventType);
});
app.MapGet("/heartrate-sseitem", (CancellationToken token) =>
{
    var eventType = "heartrate";
    async IAsyncEnumerable<SseItem<int>> GetHeartRate(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var heartRate = Random.Shared.Next(60, 100);
            yield return new SseItem<int>(heartRate, eventType: eventType)
            {
                ReconnectionInterval = TimeSpan.FromMinutes(1)
            };
            await Task.Delay(2000, cancellationToken);
        }
    }

    return TypedResults.ServerSentEvents(GetHeartRate(token), eventType);
});
app.MapGet("/heartbeat-json", (CancellationToken cancellationToken) =>
{
    async IAsyncEnumerable<HearRate> GetHeartRate(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var heartRate = Random.Shared.Next(60, 100);
            yield return HearRate.Create(heartRate);
            await Task.Delay(2000, cancellationToken);
        }
    }

    var eventType = "heartrate";
    return TypedResults.ServerSentEvents(GetHeartRate(cancellationToken), eventType);
});

app.Run();

public record HearRate(DateTime Timestamp, int HeartRate)
{
    public static HearRate Create(int heartRate) => new(DateTime.UtcNow, heartRate);
}

