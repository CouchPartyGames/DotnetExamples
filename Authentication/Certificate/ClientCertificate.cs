#!/usr/bin/env -S dotnet run

var clientCertificate = X509Certificate2.CreateFromPemFile(pem);

    // Add Certificate to 
var handler = new HttpClientHandler();
handler.ClientCertificates.Add(clientCertificate);