#!/usr/bin/env dotnet

Console.WriteLine("Hello World!");
Environment.ExitCode = (int)ExitCodes.Failure;

enum ExitCodes
{
    Success = 0,
    Failure = 1
}
