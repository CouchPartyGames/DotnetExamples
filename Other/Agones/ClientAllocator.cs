#!/usr/bin/env dotnet 

// Purpose
// Client that attempts to allocate a game server from Agones
var creds = new SslCredentials(serverCa, new KeyCertificatePair(clientCert, clientKey));
var channel = new Channel(externalIp + ":443", creds);
var client = new AllocationService.AllocationServiceClient(channel);

try {
    var response = await client.AllocateAsync(new AllocationRequest { 
        Namespace = namespaceArg,
        MultiClusterSetting = new Allocation.MultiClusterSetting {
            Enabled = multicluster,
        }
    });
    Console.WriteLine(response);
} 
catch(RpcException e)
{
    Console.WriteLine($"gRPC error: {e}");
}

public sealed class AgonesServiceOptions
{
    public string Host { get; set; }
    public string ClientCert { get; set; }
    public string  ClientKey { get; set; }
    public string ClientCA { get; set; }
}