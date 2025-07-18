#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.FeatureManagement.AspNetCore@4.2.1

using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFeatureManagement();
var app = builder.Build();
app.MapGet("/", async (IFeatureManager featureManager) =>
{
    if (await featureManager.IsEnabledAsync(FeatureFlags.ClipArticleContent))
    {
        return "Feature is enabled";
    }
    return "Hello World!";
});
app.Run();


// Using constants
public static class FeatureFlags
{
    public const string ClipArticleContent = "ClipArticleContent";
}
