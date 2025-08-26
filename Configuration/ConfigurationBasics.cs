#!/usr/bin/env dotnet


Configuration configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var clientId = configuration.GetValue<string>("ClientId"); 
var clientSecret = configuration["ClientSecret"];

// Nested 
var keycloakClientId = configuration.GetValue<string>("Authentication:Keycloak:ClientId");


// Section
var section = configuration.GetSection("Authentication");
var requiredSection = configuration.GetRequiredSection("Authentication");