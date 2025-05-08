using System;
using FF.AI.Common.Constants;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Models;
using FF.AI.Common.Providers;
using Microsoft.Extensions.Logging;

namespace FF.AI.Common.Services;

public class AiChatAssistant(
    IAssistant<OllamaProvider> _ollamaAssistant,
    IAssistant<DeepSeekProvider> _deepSeekAssistant,
    ILogger<AiChatAssistant> _logger
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

    public async Task<List<ChatProvider>> GetProviderAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task<ChatProvider>>
        {
            _ollamaAssistant.GetProviderAsync(cancellationToken),
            _deepSeekAssistant.GetProviderAsync(cancellationToken)
        };

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        var allTasks = Task.WhenAll(tasks);

        if (await Task.WhenAny(allTasks, timeoutTask) == allTasks && !allTasks.IsFaulted)
        {
            return (await allTasks).ToList();
        }

        var results = new List<ChatProvider>();
        foreach (var task in tasks)
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                try
                {
                    results.Add(await task);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error getting provider");
                }
            }
        }

        return results;
    }
}
