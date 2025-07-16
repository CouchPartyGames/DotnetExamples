#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// .NET 10 added Server Side Events
//
// SSE is a unidirectional channel from the server to a client where a client can subscribe to events
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.typedresults.serversentevents?view=aspnetcore-10.0
// https://khalidabuhakmeh.com/server-sent-events-in-aspnet-core-and-dotnet-10
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/orders", (FoodService foods, CancellationToken token) =>
    TypedResults.ServerSentEvents(
        foods.GetCurrent(token),
        eventType: "order")
);
app.Run();