namespace FF.AI.Common.Providers;

public class GeminiProvider : IProvider
{
    private readonly string? _apiKey;
    private readonly string _apiUrl;
    public GeminiProvider(IConfiguration configuration)
    {
        _apiKey = configuration["Gemini:ApiKey"];
        _apiUrl = configuration["Gemini:ApiUrl"] ?? "https://generativelanguage.googleapis.com/v1beta";
    }
    public string ProviderName => ProviderList.Gemini;
    public string ChatEndpoint => _apiUrl + "/models";
    public string? ApiKey => _apiKey;
    public string? ListModelsEndpoint => "";
}

