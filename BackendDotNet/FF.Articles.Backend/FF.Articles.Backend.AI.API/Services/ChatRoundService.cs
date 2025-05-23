using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace FF.Articles.Backend.AI.API.Services;

public class ChatRoundService(
    IChatRoundRepository _chatRoundRepository,
    ISessionRepository _sessionRepository,
    IAssistant _aiChatAssistant,
    ILogger<ChatRoundService> _logger
)
: BaseService<ChatRound, AIDbContext>(_chatRoundRepository, _logger), IChatRoundService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<ChatRoundDto> NewChatRound(ChatRoundCreateRequest request, UserApiDto user, CancellationToken cancellationToken = default)
    {
        var newRound = await getChatRound(request, user);
        var chatRequest = await getChatRequest(newRound.SessionId, request);

        var response = await _aiChatAssistant.ChatAsync(chatRequest, new CancellationToken());
        if (response == null)
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get response from AI");
        newRound.AssistantMessage = response.Message?.Content ?? string.Empty;
        newRound.PromptTokens = response?.ExtraInfo?.InputTokens ?? 0;
        newRound.CompletionTokens = response?.ExtraInfo?.OutputTokens ?? 0;
        newRound.TimeTaken = response?.ExtraInfo?.Duration ?? 0;

        this._logger.LogInformation($"Non-streaming response - Prompt tokens: {newRound.PromptTokens}, Completion tokens: {newRound.CompletionTokens}");

        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();

        return newRound.ToDto();
    }

    public async IAsyncEnumerable<SseDto> StreamChatRound(ChatRoundCreateRequest request, UserApiDto user, CancellationToken cancellationToken = default)
    {
        var newRound = await getChatRound(request, user);
        var chatRequest = await getChatRequest(newRound.SessionId, request);

        // Send initial data
        yield return new SseDto
        {
            Event = SseEvent.Init,
            Data = JsonSerializer.Serialize(newRound.ToDto(), _jsonOptions)
        };
        var streamingResponse = _aiChatAssistant.ChatStreamAsync(chatRequest, cancellationToken);
        if (streamingResponse == null)
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get streaming response from AI");
        //await Task.Delay(2000);
        //var streamingResponse = mockStreamResponse(chatRequest.Messages.Last().Content);
        var fullResponse = "";
        await foreach (var chatResponse in streamingResponse)
        {
            var content = chatResponse.Message?.Content;
            if (string.IsNullOrEmpty(content)) continue;
            // If this is the final message with usage info
            if (chatResponse.Event == ChatEvent.Finish)
            {
                newRound.PromptTokens = chatResponse.ExtraInfo?.InputTokens ?? 0;
                newRound.CompletionTokens = chatResponse.ExtraInfo?.OutputTokens ?? 0;
                newRound.TimeTaken = chatResponse.ExtraInfo?.Duration ?? 0;
                fullResponse = chatResponse.Message?.Content ?? string.Empty;
                this._logger.LogInformation($"Received token counts - Prompt: {newRound.PromptTokens}, Completion: {newRound.CompletionTokens}");
                break;
            }
            // Stream the content chunk
            yield return new SseDto
            {
                Event = SseEvent.Data,
                Data = chatResponse.Message?.Content ?? string.Empty
            };
        }

        // Save the completed chat round
        newRound.AssistantMessage = fullResponse;

        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();

        // Send completion data
        yield return new SseDto
        {
            Event = SseEvent.End,
            Data = JsonSerializer.Serialize(newRound.ToDto(), _jsonOptions)
        };
    }
    private async IAsyncEnumerable<ChatResponse> mockStreamResponse(string mockResponse)
    {
        var temp = "";
        for (int i = 0; i < 6; i++)
        {
            temp += $"Repeat {i + 1}: " + Environment.NewLine + mockResponse;
        }

        var chunks = temp.Split(' ');
        var delay = 100; // milliseconds between chunks

        foreach (var chunk in chunks)
        {
            await Task.Delay(delay);
            yield return new ChatResponse
            {
                Event = ChatEvent.Generate,
                Message = new Message { Content = chunk + " " }
            };
        }

        await Task.Delay(100);
        yield return new ChatResponse
        {
            Event = ChatEvent.Finish,
            Message = new Message { Content = temp },
            ExtraInfo = new()
            {
                InputTokens = 10,
                OutputTokens = 50,
                Duration = 2000
            }
        };
    }

    private async Task<ChatRound> getChatRound(ChatRoundCreateRequest request, UserApiDto user)
    {
        if (string.IsNullOrEmpty(request.UserMessage?.Trim()))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "User message is required");

        // Create or get session
        var sessionId = (await _sessionRepository.GetByIdAsync(request.SessionId ?? 0L))?.Id ?? 0L;
        if (sessionId == 0L)
        {
            var session = new Session
            {
                Id = EntityUtil.GenerateSnowflakeId(),
                UserId = user.UserId,
                TimeStamp = request.SessionTimeStamp,
            };
            sessionId = await _sessionRepository.CreateAsync(session);
            await _sessionRepository.SaveChangesAsync();
        }

        // Create new chat round
        var newRound = new ChatRound
        {
            Id = EntityUtil.GenerateSnowflakeId(),
            SessionId = sessionId,
            UserMessage = request.UserMessage,
            Model = request.Model ?? "",
            Provider = request.Provider ?? "",
            IsActive = true,
        };
        return newRound;
    }
    private async Task<ChatRequest> getChatRequest(long sessionId, ChatRoundCreateRequest request)
    {
        var chatRounds = await _chatRoundRepository.GetChatsBySessionId(sessionId);
        var historyMessages = chatRounds
            .Where(c => c.IsActive)
            .Where(c => request.HistoryChatRoundIds == null || request.HistoryChatRoundIds.Count == 0 || request.HistoryChatRoundIds.Contains(c.Id))
            .SelectMany(c => c.ToMessages())
            .ToList();
        var chatRequest = new ChatRequest
        {
            Model = request.Model ?? "deepseek-chat",
            Provider = request.Provider ?? ProviderList.DeepSeek,
            Messages = [.. historyMessages, Message.User(request.UserMessage)],
        };
        return chatRequest;
    }



    #region Chat Management

    public async Task<bool> DisableChatRound(List<long> ids) => await SwitchChatRoundStatus(ids, false);

    public async Task<bool> EnableChatRound(List<long> ids) => await SwitchChatRoundStatus(ids, true);

    private async Task<bool> SwitchChatRoundStatus(List<long> ids, bool isActive)
    {
        var chatRounds = await _chatRoundRepository.GetByIdsAsync(ids);
        if (chatRounds == null || !chatRounds.Any())
            return true;

        foreach (var chatRound in chatRounds)
        {
            chatRound.IsActive = isActive;
            await _chatRoundRepository.UpdateAsync(chatRound);
        }

        await _chatRoundRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(List<long> ids)
    {
        if (ids == null || !ids.Any())
            return true;

        await _chatRoundRepository.DeleteBatchAsync(ids);
        await _chatRoundRepository.SaveChangesAsync();

        return true;
    }

    public async Task<long> GetUserIdByChatRoundId(List<long> ids)
    {
        var chatRounds = await _chatRoundRepository.GetByIdsAsync(ids);
        if (chatRounds == null || !chatRounds.Any())
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Chat rounds not found");

        var sessionIds = chatRounds.Select(c => c.SessionId).Distinct().ToList();
        var sessions = await _sessionRepository.GetByIdsAsync(sessionIds);
        if (sessions == null || !sessions.Any())
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Sessions not found");

        var userIds = sessions.Select(s => s.UserId).Distinct().ToList();
        if (userIds == null || !userIds.Any() || userIds.Count != 1)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Invalid user id");

        return userIds.FirstOrDefault();
    }

    #endregion
}