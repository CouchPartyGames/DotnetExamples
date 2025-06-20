#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package AWSSDK.Extensions.NETCore.Setup@4.*
#:package AWSSDK.SimpleEmail@4.*

var builder = WebApplication.CreateBuilder();
//builder.Services.AddDefaultAWSOptions();
builder.Services.AddAWSService<IAmazonSimpleEmailService>();
var app = builder.Build();

app.MapPost("/sendemail", () =>
{
    var email = "test@test.com"
    var request = new SendEmailRequest
    {
        Source = "sample",
        Destination = new Destination
        {
           ToAddresses = [ email ] 
        },
        Message =
        {
            Subject = "This is a the subject",
            Body = "This is the body"
        }
    };

    var response = await emailService.SendEmailAsync(request);

});
app.Run();