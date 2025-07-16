#!/bin/bash

openssl genrsa -out example.key 4096
openssl rsa -in example.key -pubout -out example.crt
openssl req -new -key example.key -out example.csr



