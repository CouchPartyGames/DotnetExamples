#!/usr/bin/env dotnet

using System.Security.Cryptography.X509Certificates;

try
{
    var clientCertificate = X509Certificate2.CreateFromPemFile("./client.certificate.pem", "./client.private.key.pem");
    
    var handler = new HttpClientHandler();
    handler.ClientCertificates.Add(clientCertificate); 
    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    /*handler.ServerCertificateCustomValidationCallback = 
        (httpRequestMessage, cert, cetChain, policyErrors) =>
        {
            //httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            
            return true;
        };*/
    
    using HttpClient client = new HttpClient(handler);
    client.BaseAddress = new Uri("https://example.com:5001/");
    HttpResponseMessage response = await client.GetAsync("protected");
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody);
    } 
    else
    {
        Console.WriteLine($"Failed: {response.StatusCode}");
		Console.WriteLine("Failed Request");
	}
}
catch(Exception exception)
{
    Console.WriteLine(exception);
    return 1;
}

return 0;
