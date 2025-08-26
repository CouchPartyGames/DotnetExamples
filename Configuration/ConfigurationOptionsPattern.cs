
// Bind multiple values to an object

// Method 1
var apiOptions = configuration.GetSection(ExternalApiOptions.SectionName).Get<ExternalApiOptions>();

// Method 2
var apiOptions = new ExternalApiOptions();
configuration.GetSection(ExternalApiOptions.SectionName).Bind(apiOptions);

// Method 3 - Preferred approach
services.Configure<ExternalApiOptions>(configuration);

public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApi";
    
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}