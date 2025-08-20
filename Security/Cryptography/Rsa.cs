#!/usr/bin/env dotnet 

using System.Security.Cryptography;

int size = 4096;
    // Creates a new instance of the RSA class, generating a public/private key pair
using var rsa = RSA.Create(size);
var privateKey = rsa.ExportRSAPrivateKey();
var publicKey = rsa.ExportRSAPublicKey();

Console.WriteLine("RSA Private key:\n{0}", Convert.ToBase64String(privateKey));
Console.WriteLine("RSA Public key:\n{0}", Convert.ToBase64String(publicKey));


public sealed class RsaUtilties
{
    
    
}