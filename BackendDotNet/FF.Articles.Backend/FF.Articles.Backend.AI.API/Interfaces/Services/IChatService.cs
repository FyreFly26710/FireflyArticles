using System;

namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface IChatService
{
    Task<string> ChatAsync(string message, long conversationId, HttpRequest httpRequest, CancellationToken cancellationToken = default);

}
