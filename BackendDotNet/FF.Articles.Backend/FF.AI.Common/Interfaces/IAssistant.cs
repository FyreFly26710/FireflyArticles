namespace FF.AI.Common.Interfaces;

public interface IAssistant<TProvider> where TProvider : IProvider
{
    Task<ChatProvider> GetProviderAsync(CancellationToken cancellationToken);
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, CancellationToken cancellationToken);
}

public interface IAssistant
{
    Task<List<ChatProvider>> GetProviderAsync(CancellationToken cancellationToken);
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, CancellationToken cancellationToken);
}