using System;
using FF.AI.Common.Constants;
using FF.AI.Common.Models;
using FF.AI.Common.Providers;

namespace FF.AI.Common.Interfaces;

public interface IAssistant<TProvider> where TProvider : IProvider
{
    Task<List<string>> GetModelsAsync(CancellationToken cancellationToken);
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, CancellationToken cancellationToken);
}
