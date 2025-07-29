#!/usr/bin/env dotnet
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
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

/*
builder.Services.Configure<KestrelServerOptions>(opts =>
{
    opts.ConfigureHttpsDefaults(options =>
    {
            // Kestrel controls client certificate negotiation
        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        options.CheckCertificateRevocation = false;
    });
});
*/
builder.WebHost.ConfigureKestrel((context, options) =>
{
    // ConfigureHttpsDefaults *MUST* come before opening the Port/Listen
    options.ConfigureHttpsDefaults(listenOptions =>
    {
        listenOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        listenOptions.CheckCertificateRevocation = false;
        //listenOptions.SslProtocols = SslProtocols.Tls13;
    });
    options.ListenAnyIP(5001, listenOptions =>
    {
        var x509Cert = X509Certificate2.CreateFromPemFile("./server.certificate.pem", "./server.private.key.pem");
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps(x509Cert);
    });
});


builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(opts =>
    {
        opts.ValidateCertificateUse = true;
        opts.ValidateValidityPeriod = true;
        opts.RevocationMode = X509RevocationMode.NoCheck;
        opts.RevocationFlag = X509RevocationFlag.EntireChain;

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.certificate.certificatetypes?view=aspnetcore-8.0
        opts.AllowedCertificateTypes = CertificateTypes.All;

        opts.Events = new CertificateAuthenticationEvents
        {
            // These Events ONLY run if Authorization is setup (AddAuthorization & UseAuthorization)
            OnCertificateValidated = ctx =>
            {
                Console.WriteLine("OnCertificateValidated");
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, ctx.ClientCertificate.Subject,
                        ClaimValueTypes.String, ctx.Options.ClaimsIssuer),
                    new Claim(ClaimTypes.Name, ctx.ClientCertificate.Subject,
                        ClaimValueTypes.String, ctx.Options.ClaimsIssuer)
                };

                ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ctx.Scheme.Name));
                ctx.Success();

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("OnAuthenticationFailed");
                ctx.Fail("Failed to authenticate");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine("OnChallenge");
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
        return $"Accepted: ${clientCertificate.Subject} with {clientCertificate.Thumbprint}";
    }

    return "unable to get certificate";
}).RequireAuthorization();

app.Run();