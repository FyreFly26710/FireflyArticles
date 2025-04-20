using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.Chats;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.AI.API.MapperExtensions.Chats;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.Services;
using FF.Articles.Backend.AI.Models;
using System.Diagnostics;
using FF.Articles.Backend.AI.Constants;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json.Serialization;

namespace FF.Articles.Backend.AI.API.Services;

public class ChatRoundService(
    IChatRoundRepository _chatRoundRepository,
    ISessionRepository _sessionRepository,
    IDeepSeekClient _deepSeekClient,
    ILogger<ChatRoundService> _logger
)
: BaseService<ChatRound, AIDbContext>(_chatRoundRepository, _logger), IChatRoundService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public async Task<ChatRoundDto> NewChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        var newRound = await getChatRound(request, httpRequest);
        var chatRequest = await getChatRequest(newRound.SessionId, request);

        var stopwatch = Stopwatch.StartNew();
        var response = await _deepSeekClient.ChatAsync(chatRequest, new CancellationToken());
        if (response == null)
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get response from DeepSeek AI");
        stopwatch.Stop();

        newRound.AssistantMessage = response.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
        newRound.PromptTokens = response?.Usage?.PromptTokens ?? 0;
        newRound.CompletionTokens = response?.Usage?.CompletionTokens ?? 0;
        newRound.TimeTaken = (int)stopwatch.Elapsed.TotalMilliseconds;

        _logger.LogInformation($"Non-streaming response - Prompt tokens: {newRound.PromptTokens}, Completion tokens: {newRound.CompletionTokens}");

        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();

        return newRound.ToDto();
    }

    public async IAsyncEnumerable<SseDto> StreamChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        var newRound = await getChatRound(request, httpRequest);
        var chatRequest = await getChatRequest(newRound.SessionId, request);

        // Send initial data
        yield return new SseDto
        {
            Event = SseEvent.Init,
            Data = JsonSerializer.Serialize(newRound.ToDto(), _jsonOptions)
        };

        var responseBuilder = new StringBuilder();
        var stopwatch = Stopwatch.StartNew();
        int promptTokens = 0;
        int completionTokens = 0;

        var streamingResponse = _deepSeekClient.ChatStreamAsync(chatRequest, new CancellationToken());
        if (streamingResponse == null)
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get streaming response from DeepSeek AI");

        await foreach (var json in streamingResponse)
        {
            if (json == "[DONE]") break;
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var chatResponse = JsonSerializer.Deserialize<ChatResponse>(json, jsonOptions);
            if (chatResponse == null) continue;

            var content = chatResponse.Choices.FirstOrDefault()?.Delta?.Content;
            var finishReason = chatResponse.Choices.FirstOrDefault()?.FinishReason;
            // Only process tokens when we get the final message (indicated by finish_reason being "stop")
            if (finishReason == "stop")
            {
                completionTokens = chatResponse.Usage?.CompletionTokens ?? 0;
                promptTokens = chatResponse.Usage?.PromptTokens ?? 0;
                _logger.LogInformation($"Received token counts - Prompt: {promptTokens}, Completion: {completionTokens}");
            }

            if (content == null || string.IsNullOrEmpty(content)) continue;

            responseBuilder.Append(content);

            yield return new SseDto { Event = SseEvent.Data, Data = content };
        }
        stopwatch.Stop();

        // Save the completed chat round
        newRound.AssistantMessage = responseBuilder.ToString();
        newRound.TimeTaken = (int)stopwatch.Elapsed.TotalMilliseconds;
        newRound.PromptTokens = promptTokens;
        newRound.CompletionTokens = completionTokens;

        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();

        // Send completion data
        yield return new SseDto
        {
            Event = SseEvent.End,
            Data = JsonSerializer.Serialize(newRound.ToDto(), _jsonOptions)
        };

    }

    private async Task<ChatRound> getChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest)
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
                UserId = UserUtil.GetUserId(httpRequest),
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
            Messages = [.. historyMessages, Message.NewUserMessage(request.UserMessage)],
            Temperature = 0.7,
            MaxTokens = 2000,
            Stream = false,
            StreamOptions = new StreamOptions { IncludeUsage = true }
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