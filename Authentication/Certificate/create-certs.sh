#!/bin/bash

mkcert -install

  # Create Server and Client Certificates
  # Server should be used for Kestrel
  # Client should be used for HTTP Requests (client)
mkcert -cert-file server.certificate.pem -key-file server.private.key.pem example.com localhost 127.0.0.1 ::1
mkcert -client -cert-file client.certificate.pem -key-file client.private.key.pem example.com localhost 127.0.0.1 ::1

  # Print Location of CA Root
mkcert -CAROOT