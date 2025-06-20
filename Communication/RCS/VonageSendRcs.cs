#!/usr/bin/env -S dotnet run
#:package Vonage@8.0.1

// https://developer.vonage.com/en/messages/code-snippets/rcs/send-text?source=messages&lang=dotnet
using Vonage;
using Vonage.Messages;
using Vonage.Messages.Rcs;

var messageToNumber = "";
var senderId = "";

try {
	var credentials = Credentials.FromAppIdAndPrivateKeyPath("", "");
	var vonageClient = new VonageClient(credentials);
	var request = new RcsTextRequest
	{
		To = messageToNumber,
		From = senderId,
		Text = "This is an RCS text message sent via the Vonage Messages API",
	};
	
	var response = await vonageClient.MessagesClient.SendAsync(request);
	Console.WriteLine($"Message UUID: {response.MessageUuid}");

} catch(Exception e) {
	Console.WriteLine($"Error sending message: {e.Message}");
}
