#!/usr/bin/env dotnet

var clientCertificate = X509Certificate2.CreateFromPemFile(pem);

    // Add Certificate to 
var handler = new HttpClientHandler();
handler.ClientCertificates.Add(clientCertificate);