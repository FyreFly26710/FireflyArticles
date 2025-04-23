using System;
using Microsoft.Extensions.Configuration;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Constants;
namespace FF.AI.Common.Providers;

public class DeepSeekProvider : IProvider
{
    private readonly string? _apiKey;
    private readonly string _apiUrl;
    public DeepSeekProvider(IConfiguration configuration)
    {
        _apiKey = configuration["DeepSeek:ApiKey"];
        _apiUrl = configuration["DeepSeek:ApiUrl"] ?? "https://api.deepseek.com/v1";
    }
    public string ProviderName => ProviderList.DeepSeek;
    public string ChatEndpoint => _apiUrl + "/chat/completions";
    public string? ApiKey => _apiKey;
    public string? ListModelsEndpoint => _apiUrl + "/models";
}