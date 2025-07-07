#!/usr/bin/env -S dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.Certificate@10.0.0-preview*

// Certificate Authentication
//
// Certificate authentication happens at the TLS level, long before it ever gets to ASP.NET Core
// You must configure your server (in this case, Kestrel) for certificate authentication.
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-10.0
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for Client Certificates
builder.Services.Configure<KestrelServerOptions>(opts =>
{
    opts.ConfigureHttpsDefaults(options =>
    {
        options.ServerCertificate = new X509Certificate2();
        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        options.CheckCertificateRevocation = false;
        options.SslProtocols = SslProtocols.Tls13;
    });
});


builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(opts =>
    {
        opts.ValidateValidityPeriod = true;
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.certificate.certificatetypes?view=aspnetcore-8.0
        opts.AllowedCertificateTypes = CertificateTypes.All;
        opts.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = ctx =>
            {
                return Task.CompletedTask;
            }
        };

    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", (HttpContext ctx) =>
{
    var clientCertificate = ctx.Connection.ClientCertificate;
    if (clientCertificate is not null)
    {
        //clientCertificate.Subject;
        //clientCertificate.Thumbprint;
        return "Hello World!";
    }

    return "failed";
}).RequireAuthorization();
app.Run();
