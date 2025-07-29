#!/bin/bash


mkcert -install
mkcert -client -cert-file certificate.pem -key-file private.key.pem example.com localhost 127.0.0.1 ::1
