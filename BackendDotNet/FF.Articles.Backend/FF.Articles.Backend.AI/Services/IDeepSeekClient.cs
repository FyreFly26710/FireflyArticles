using System;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;

namespace FF.Articles.Backend.AI.Services;

public interface IDeepSeekClient
{
    Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<Choice>? ChatStreamAsync(ChatRequest request, CancellationToken cancellationToken);
    string? ErrorMsg { get; }
}
