#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Stripe.net@48.2.0

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
        //return Results.Json(session);
    }
    catch (StripeException e)
    {
        return Results.BadRequest(e.Message);
    }
});
app.MapGet("/success", () => "Success!");
app.MapGet("/cancel", () => "Cancel!");

// stripe login
// stripe listen -e checkout.session.completed --forward-to http://localhost:5000/webhook
// stripe trigger checkout.session.completed
app.MapPost("/webhook", async (HttpRequest request) => {
	using StreamReader stream = new StreamReader(request.Body);
	string json = await stream.ReadToEndAsync();

	try {
		var stripeData = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_KEY"));
		Console.WriteLine(stripeData);
	} catch(StripeException e) {
		return Results.BadRequest(e.Message);
	}

	return Results.Ok();
});

app.Run();
