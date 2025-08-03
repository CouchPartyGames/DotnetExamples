#!/usr/bin/env dotnet

// Another Option is using GUID
// Use CreateVersion7 for better database lookup
var apiKey = Guid.CreateVersion7();
string base64String = Convert.ToBase64String(apiKey.ToByteArray());

Console.WriteLine($"New Api Key (guid): {apiKey}");
Console.WriteLine($"New Api Key (base64): {base64String}");
