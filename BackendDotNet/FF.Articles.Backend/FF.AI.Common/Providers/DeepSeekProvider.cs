using System;
using Microsoft.Extensions.Configuration;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Constants;
namespace FF.AI.Common.Providers;

public class DeepSeekProvider : IProvider
{
    private readonly string? _apiKey;
    private readonly string? _chatEndpoint;
    private readonly string? _listModelsEndpoint;
    public DeepSeekProvider(IConfiguration configuration)
    {
        _apiKey = configuration["ApiKey:DeepSeek"];
        _chatEndpoint = configuration["ChatEndpoint:DeepSeek"];
        _listModelsEndpoint = configuration["ListModelsEndpoint:DeepSeek"];
    }
    public string ProviderName => ProviderList.DeepSeek;
    public string ChatEndpoint => _chatEndpoint ?? "https://api.deepseek.com/v1/chat/completions";
    public string? ApiKey => _apiKey;
    public string? ListModelsEndpoint => _listModelsEndpoint ?? "https://api.deepseek.com/v1/models";
}