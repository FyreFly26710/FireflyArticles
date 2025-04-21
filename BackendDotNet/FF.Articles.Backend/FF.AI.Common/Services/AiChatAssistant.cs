using System;
using FF.AI.Common.Constants;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Models;
using FF.AI.Common.Providers;

namespace FF.AI.Common.Services;

public class AiChatAssistant(
    IAssistant<OllamaProvider> _ollamaAssistant,
    IAssistant<DeepSeekProvider> _deepSeekAssistant
    ) : IAssistant
{
    public Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        return request.Provider.ToLower() switch
        {
            ProviderList.Ollama => _ollamaAssistant.ChatAsync(request, cancellationToken),
            ProviderList.DeepSeek => _deepSeekAssistant.ChatAsync(request, cancellationToken),
            _ => throw new ArgumentException($"Provider {request.Provider} not supported")
        };
    }

    public IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        return request.Provider.ToLower() switch
        {
            ProviderList.Ollama => _ollamaAssistant.ChatStreamAsync(request, cancellationToken),
            ProviderList.DeepSeek => _deepSeekAssistant.ChatStreamAsync(request, cancellationToken),
            _ => throw new ArgumentException($"Provider {request.Provider} not supported")
        };
    }

    /// <summary>
    /// Todo: Cache this
    /// </summary>
    public async Task<List<ChatProvider>> GetProviderAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task<ChatProvider>>
        {
            _ollamaAssistant.GetProviderAsync(cancellationToken),
            _deepSeekAssistant.GetProviderAsync(cancellationToken)
        };
        var providers = await Task.WhenAll(tasks);
        return providers.ToList();
    }
}
