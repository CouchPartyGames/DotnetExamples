#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*
#:package Microsoft.Extensions.ApiDescription.Server@10.*-*
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.*-*
#:package Scalar.AspNetCore@2.7.*


// https://damienbod.com/2024/08/06/implementing-an-asp-net-core-api-with-net-9-and-openapi/
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-9.0
// https://github.com/scalar/scalar/issues/4055
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
    options.AddDocumentTransformer<OAuth2SecuritySchemeTransformer>();
    options.AddDocumentTransformer<OpenIdConnectSecuritySchemeTransformer>();
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => "Hello World!");
app.Run();

// https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.models.securityschemetype
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.models.openapisecurityscheme
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.models.parameterlocation
public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",              // "bearer" refers to the header name here
                    In = ParameterLocation.Header,      // Expects in HTTP Header
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;
        }
        document.Info = new()
        {
            Title = "My API Bearer scheme",
            Version = "v1",
            Description = "API for Damien"
        };
    }
}

public sealed class ApiKeySecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var apiKeyScheme = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["ApiKey"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "X-API-Key", // Header name
                In = ParameterLocation.Header,              // Expects in HTTP Header
                Description = "API Key authentication"
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        
        foreach (var scheme in apiKeyScheme)
        {
            document.Components.SecuritySchemes[scheme.Key] = scheme.Value;
        }

        return Task.CompletedTask;
    }
}

public sealed class OAuth2SecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var oAuth2Scheme = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["OAuth2"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://example.com/oauth/authorize"),
                        TokenUrl = new Uri("https://example.com/oauth/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["read"] = "Read access",
                            ["write"] = "Write access",
                            ["admin"] = "Admin access"
                        }
                    }
                },
                Description = "OAuth2 authorization code flow"
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        
        foreach (var scheme in oAuth2Scheme)
        {
            document.Components.SecuritySchemes[scheme.Key] = scheme.Value;
        }

        return Task.CompletedTask;
    }
}

public sealed class OpenIdConnectSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var oidcScheme = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["OpenIdConnect"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri("https://example.com/.well-known/openid_configuration"),
                Description = "OpenID Connect authentication"
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        
        foreach (var scheme in oidcScheme)
        {
            document.Components.SecuritySchemes[scheme.Key] = scheme.Value;
        }

        return Task.CompletedTask;
    }
}