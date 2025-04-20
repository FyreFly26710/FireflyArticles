using System;
using Microsoft.Extensions.Configuration;
using FF.AI.Common.Interfaces;

namespace FF.AI.Common.Providers;
public class OllamaProvider : IProvider
{
    private readonly string? _chatEndpoint;
    private readonly string? _listModelsEndpoint;
    public OllamaProvider(IConfiguration configuration)
    {
        _chatEndpoint = configuration["ChatEndpoint:Ollama"];
        _listModelsEndpoint = configuration["ListModelsEndpoint:Ollama"];
    }
    public string ChatEndpoint => _chatEndpoint ?? "http://localhost:11434/api/chat";
    public string? ApiKey => null;
    public string? ListModelsEndpoint => _listModelsEndpoint ?? "http://localhost:11434/api/tags";
}
