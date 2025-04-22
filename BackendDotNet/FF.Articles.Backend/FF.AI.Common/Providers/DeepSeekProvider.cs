using System;
using Microsoft.Extensions.Configuration;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Constants;
namespace FF.AI.Common.Providers;

public class DeepSeekProvider : IProvider
{
    private readonly string? _apiUrl;
    private readonly string _baseUrl;
    public DeepSeekProvider(IConfiguration configuration)
    {
        _apiUrl = configuration["DeepSeek:ApiKey"];
        _baseUrl = configuration["DeepSeek:ApiUrl"] ?? "https://api.deepseek.com/v1/";
    }
    public string ProviderName => ProviderList.DeepSeek;
    public string ChatEndpoint => _baseUrl + "chat/completions";
    public string? ApiKey => _apiUrl;
    public string? ListModelsEndpoint => _baseUrl + "models";
}