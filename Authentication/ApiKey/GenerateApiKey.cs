#!/usr/bin/env dotnet

// Purpose: Generate an API Key to used for API Key Authentication
using System.Security.Cryptography;

// Slower than System.Random() but prefered b/c of strong cryptographic random values
var key = new byte[32];
using (var generator = RandomNumberGenerator.Create())
    generator.GetBytes(key);
string apiKey = Convert.ToBase64String(key);


// Another Option is using GUID
var apiKey2 = Guid.CreateVersion7();

Console.WriteLine($"New Api Key (base64): {apiKey}");
