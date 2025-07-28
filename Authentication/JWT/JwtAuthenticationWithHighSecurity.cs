#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Authentication.JwtBearer@10.0.0-preview.*
#:package System.IdentityModel.Tokens.Jwt@8.13.*

// Default AddJwtBearer() Defaults to
// - 'alg': 'none' which bypasses signature verification
// - default validation can accept unsigned token
// Attackers can forge any user identity

// Recommeneded Changes
// - Explicit Algorithm Validation
//   - Validate Signing Key
//   - Set Valid Algorithms
// - Reject 'None' Algorithm
// - Validate issuer and audience
// - Implement token revocation
// - Log all authentication attempts
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            // Explicit Algorithm Validation
            ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidAlgorithms = new[]
            {
                SecurityAlgorithms.HmacSha256
            },

            // Reject 'None' Algorithm
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () =>
{
    return Results.Content("<h1>Hello World!</h1><a href=/login>Login</a>&nbsp;<a href=/protected>Protected Page</a>",
        "text/html");
});
app.MapGet("/login", (HttpContext ctx) =>
{
    ctx.SignInAsync(new ClaimsPrincipal(new []
    {
        new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@test.com"),
        ])
    }));
    
    return Results.Text("You have logged in and received a cookie");
});
app.MapGet("/protected", () => "Secret")
    .RequireAuthorization();
app.MapGet("/user", (ClaimsPrincipal principal) =>
{
    var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
    return Results.Ok(claims);
}).RequireAuthorization();

app.Run();


public sealed class JwtToken(string Audience, string Issuer)
{
    private JsonWebTokenHandler _tokenHandler = new();
    
    public string CreateAndSignToken()
    {
        var credentials = new SigningCredentials(new ECDsaSecurityKey(key), "ES256");
        List<Claim> claims = [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Email, email)
        ];
            
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Expires = now.AddMinutes(30),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials
        };
        
        return _tokenHandler.CreateToken(tokenDescriptor);
        //return _tokenHandler.WriteToken(token);
    }
}