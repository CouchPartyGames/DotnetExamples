#!/usr/bin/env dotnet

using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


using var ecdsa = ECDsa.Create();
var req = new CertificateRequest("cn=foobar", ecdsa, HashAlgorithmName.SHA256);
var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

var privateKey = ecdsa.ExportECPrivateKey();

Console.WriteLine("Key:\n{0}", Encoding.ASCII.GetString(privateKey));
Console.WriteLine("Cert:\n{0}", cert.ExportCertificatePem());
