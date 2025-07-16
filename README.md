# DotnetAuthentication
.NET 10 Authentication examples using dotnet run

# Requirements
.NET 10 preview 5 or greater since not project is attached to any of these examples.

Create secrets in ~/.microsoft/usersecrets/

```
dotnet user-secrets init --id dotnet-examples --project TestProject.csproj
```

# Basic

```
dotnet run JwtAuthentication.cs 
dotnet run CookieAuthentication.cs 
```

# Certificate

```
dotnet run Certificate/BasicCertificateAuthentication.cs
```

# OAuth

```
dotnet run OAuth/OAuthAuthentication.cs
dotnet run OAuth/GithubOAuthAuthentication.cs 
```

# Social

```
dotnet run Social/GoogleSocialAuthentication.cs 
dotnet run Social/FacebookSocialAuthentication.cs
dotnet run Social/TwitterSocialAuthentication.cs
dotnet run Social/MicrosoftSocialAuthentication.cs
```





Extra
```
dotnet convert project <File>

dotnet publish <File>
```