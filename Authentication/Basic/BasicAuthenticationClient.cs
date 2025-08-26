#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk

// Basic Authentication Client Example
// Sends HTTP requests with Basic Authentication headers to test the BasicAuthentication.cs server
using System.Text;

var client = new HttpClient();
var baseUrl = "http://localhost:5000"; // Default HTTPS port for ASP.NET Core

Console.WriteLine("Basic Authentication Client");
Console.WriteLine("===========================");

// Test 1: Call public endpoint without authentication
Console.WriteLine("\n1. Testing public endpoint (/):");
await TestEndpoint(client, $"{baseUrl}/", null);

// Test 2: Call protected endpoint without authentication
Console.WriteLine("\n2. Testing protected endpoint (/user) without auth:");
await TestEndpoint(client, $"{baseUrl}/user", null);

// Test 3: Call protected endpoint with valid credentials
Console.WriteLine("\n3. Testing protected endpoint (/user) with Basic Auth: (testuser:testpass)");
var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass"));
await TestEndpoint(client, $"{baseUrl}/user", $"Basic {credentials}");

// Test 4: Call protected endpoint with different credentials
Console.WriteLine("\n4. Testing with different credentials: (admin:secret)");
var altCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:secret"));
await TestEndpoint(client, $"{baseUrl}/user", $"Basic {altCredentials}");

client.Dispose();

static async Task TestEndpoint(HttpClient client, string url, string? authHeader)
{
    try
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        if (!string.IsNullOrEmpty(authHeader))
        {
            request.Headers.Add("Authorization", authHeader);
            Console.WriteLine($"  Authorization: {authHeader}");
        }
        else
        {
            Console.WriteLine("  No authorization header");
        }
        
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"  Status: {(int)response.StatusCode} {response.StatusCode}");
        Console.WriteLine($"  Response: {content}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  Error: {ex.Message}");
        Console.WriteLine("  Note: Make sure the BasicAuthentication.cs server is running on http://localhost:5000");
    }
}
