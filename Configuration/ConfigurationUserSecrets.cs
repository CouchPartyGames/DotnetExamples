#!/usr/bin/env dotnet
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.*-*

using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
    
string? apiKey = config["MyService:ApiKey"];
Console.WriteLine(apiKey);