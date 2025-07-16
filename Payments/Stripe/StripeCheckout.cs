#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web
#:package Stripe.net@48.2.0
#:property UserSecretsId dotnet-examples

using System;
using Stripe;
using Stripe.Checkout;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapPost("/checkout", async () => {

    StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
    var options = new SessionCreateOptions
    {
        SuccessUrl = "http://localhost:5000/success",
        CancelUrl = "http://localhost:5000/cancel",
        PaymentMethodTypes = [ "card" ],
        Mode = "payment",
        LineItems = [
            new SessionLineItemOptions
            {
                Price = "price_1RWcneDbbyfyaE6FWCPr6lWQ",
                Quantity = 1,
            }
        ]
    };
    try
    {
        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return Results.Redirect(session.Url);
    }
    catch (StripeException e)
    {
        return Results.BadRequest(e.Message);
    }
});
app.MapGet("/success", () => "Success!");
app.MapGet("/cancel", () => "Cancel!");
app.Run();