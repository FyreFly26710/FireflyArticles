using System;
using Microsoft.Extensions.Configuration;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Constants;
namespace FF.AI.Common.Providers;
public class OllamaProvider : IProvider
{
    private readonly string _apiUrl;
    public OllamaProvider(IConfiguration configuration)
    {
        _apiUrl = configuration["Ollama:ApiUrl"] ?? "http://localhost:11434";
    }
    public string ProviderName => ProviderList.Ollama;
    public string ChatEndpoint => _apiUrl + "/api/chat";
    public string? ApiKey => null;
    public string ListModelsEndpoint => _apiUrl + "/api/tags";
}
