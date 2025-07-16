#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Stripe.net@48.2.0

// Purpose: Create Stripe Webhook that receives messages from Stripe
//
// Install stripe cli tool for simple debugging
// stripe login
// stripe listen -e checkout.session.completed --forward-to http://localhost:5000/webhook
// stripe trigger checkout.session.completed

using System;
using Stripe;
using Stripe.Checkout;


var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.MapPost("/webhook", async (HttpRequest request) => {
    using StreamReader stream = new StreamReader(request.Body);
    string json = await stream.ReadToEndAsync();

    try {
        var stripeData = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], 
            Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_KEY"));
        Console.WriteLine(stripeData);
    } catch(StripeException e) {
        return Results.BadRequest(e.Message);
    }

    return Results.Ok();
});
app.Run();