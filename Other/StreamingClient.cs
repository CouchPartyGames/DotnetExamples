#!/usr/bin/env dotnet
var httpClient = new HttpClient();
string fileToDownload = "https://"

string fileName = fileToDownload.Split('/').Last();

