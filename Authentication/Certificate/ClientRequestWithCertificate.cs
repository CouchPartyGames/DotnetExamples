#!/usr/bin/env dotnet

using System.Security.Cryptography.X509Certificates;

try
{
    var clientCertificate = X509Certificate2.CreateFromPemFile("./certificate.pem", "./private.key.pem");
    
        // Add Certificate to Request
    var handler = new HttpClientHandler();
    handler.ClientCertificates.Add(clientCertificate);
    
    HttpClient client = new HttpClient(handler);
    HttpResponseMessage response = await client.GetAsync("https://example.com:5001/protected");
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
    } else {
		Console.WriteLine("Failed Request");
	}
    
}
catch(Exception exception)
{
    Console.WriteLine(exception);
    return 1;
}

return 0;
