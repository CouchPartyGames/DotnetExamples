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


builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });
    options.ConfigureHttpsDefaults(listenOptions =>
    {
        listenOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
        listenOptions.SslProtocols = SslProtocols.Tls13;
    });
});


// Configure Kestrel for Client Certificates
/*
builder.Services.Configure<KestrelServerOptions>(opts =>
{
    opts.ConfigureHttpsDefaults(options =>
    {
            // Kestrel controls client certificate negotiation
        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        
        options.CheckCertificateRevocation = false;
        // Optional but I encourage using the latest TLS protocol
        options.SslProtocols = SslProtocols.Tls13;
    });
});
*/


builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(opts =>
    {
        opts.ValidateValidityPeriod = true;
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.certificate.certificatetypes?view=aspnetcore-8.0
        opts.AllowedCertificateTypes = CertificateTypes.All;
        opts.Events = new CertificateAuthenticationEvents
        {
            // These Events ONLY run if Authorization is setup (AddAuthorization & UseAuthorization)
            OnCertificateValidated = ctx =>
            {
                if (true)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, ctx.ClientCertificate.Subject,
                            ClaimValueTypes.String, ctx.Options.ClaimsIssuer),
                        new Claim(ClaimTypes.Name, ctx.ClientCertificate.Subject,
                            ClaimValueTypes.String, ctx.Options.ClaimsIssuer)
                    };
                    
                    ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ctx.Scheme.Name));
                    ctx.Success();
                }
                else {
                    ctx.Fail("Certificate validation failed.");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                ctx.Fail("Failed to authenticate");
                return Task.CompletedTask;
            }
        };

    })
    .AddCertificateCache(opts =>
    {
        opts.CacheSize = 1024;
        opts.CacheEntryExpiration = TimeSpan.FromMinutes(5);
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