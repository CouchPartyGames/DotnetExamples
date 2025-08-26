// Preferred Way
// https://www.youtube.com/watch?v=Vvl8w6OfDCM

services.Configure<ExternalApiSettings>(configuration);

public sealed class ExternalApiClient
{
    private readonly ExternalApiSettings _externalApiOptions;

    public ExternalApiClient(IOptions<ExternalApiSettings> externalApiOptions)
    {
        _externalApiOptions = externalApiOptions;
    }
    
    public void CallExternalApi()
    {}
}