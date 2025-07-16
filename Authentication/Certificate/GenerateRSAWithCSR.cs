#!/usr/bin/env dotnet

using System.Security.Cryptography.X509Certificates;


var keySize = 4096;
using var rsa = RSA.Create(keySize);

var request = new CertificateRequest(
	"CN=Example",
	rsa.PublicKey,
	HashAlgorithmName.SHA512,
	RSASignaturePadding.Pkcs1);

/*
request.CertificateExtensions.Add(new X509BasicConstraintsExtension(
	true/*certificateAuthority*/,
	false/*hasPathLengthConstraint*/,
	0/*pathLengthConstraint*/,
	true/*critical*/));

request.CertificateExtensions.Add(new X509KeyUsageExtension(
	X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign/*keyUsages*/,
	false/*critical*/));

request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(
	request.PublicKey/*subjectKeyIdentifier*/,
	false/*critical*/));
*/




public sealed class X509Utilties {
	

	public static string PemEncodeSigningRequest(CertificateRequest request, PkcsSignatureGenerator generator)
	{
		byte[] pkcs10 = request.CreateSigningRequest(generator);
		StringBuilder builder = new StringBuilder();

		builder.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");

		string base64 = Convert.ToBase64String(pkcs10);

		int offset = 0;
		const int LineLength = 64;

		while (offset < base64.Length)
		{
			int lineEnd = Math.Min(offset + LineLength, base64.Length);
			builder.AppendLine(base64.Substring(offset, lineEnd - offset));
			offset = lineEnd;
		 }

		 builder.AppendLine("-----END CERTIFICATE REQUEST-----");
		 return builder.ToString();
	}
}
