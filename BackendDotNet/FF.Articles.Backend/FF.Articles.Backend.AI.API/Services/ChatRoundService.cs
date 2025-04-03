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
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public async Task<ChatRoundDto> NewChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        ValidateUserMessage(request.UserMessage);

        var sessionId = await _sessionRepository.GetOrCreateSessionId(request.SessionId, 1);
        var historyMessages = await GetHistoryMessages(sessionId, request.HistoryChatRoundIds);

        var newRound = CreateNewChatRound(sessionId, request.UserMessage, request.Model);

        // Get AI response
        _logger.LogInformation("Asking DeepSeek AI for response...");
        var stopwatch = Stopwatch.StartNew();
        var response = await QueryDeepSeek(
            [.. historyMessages, Message.NewUserMessage(newRound.UserMessage)],
            false,
            cancellationToken
        );
        stopwatch.Stop();
        _logger.LogInformation("DeepSeek AI response received, Time taken: {TimeTaken}ms", stopwatch.Elapsed.TotalMilliseconds);

        // Store the response in the chat round
        newRound.AssistantMessage = response.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
        newRound.PromptTokens = response?.Usage?.PromptTokens ?? 0;
        newRound.CompletionTokens = response?.Usage?.CompletionTokens ?? 0;
        newRound.TimeTaken = (int)stopwatch.Elapsed.TotalMilliseconds;

        // Add the chat round to the conversation
        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();

        return newRound.ToDto();
    }

    public async Task StreamChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest, HttpResponse httpResponse, CancellationToken cancellationToken = default)
    {
        ValidateUserMessage(request.UserMessage);

        var sessionId = await _sessionRepository.GetOrCreateSessionId(request.SessionId, 1);
        var historyMessages = await GetHistoryMessages(sessionId, request.HistoryChatRoundIds);
        var newRound = CreateNewChatRound(sessionId, request.UserMessage, request.Model);

        // Prepare for streaming
        var responseBuilder = new StringBuilder();
        _logger.LogInformation("Streaming response builder prepared");
        var stopwatch = Stopwatch.StartNew();
        var tokenInfo = new TokenInfo();

        // Send initial data to client
        await SendInitialData(httpResponse, newRound, cancellationToken);

        try
        {
            await ProcessStreamingResponse(
                httpResponse,
                historyMessages,
                newRound,
                responseBuilder,
                tokenInfo,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            await HandleStreamingError(
                httpResponse,
                ex,
                newRound,
                responseBuilder,
                stopwatch,
                tokenInfo,
                cancellationToken
            );
            throw;
        }

        stopwatch.Stop();
        _logger.LogInformation("Streaming completed, Time taken: {TimeTaken}ms", stopwatch.Elapsed.TotalMilliseconds);
        // Save the completed chat round
        newRound.AssistantMessage = responseBuilder.ToString();
        newRound.TimeTaken = (int)stopwatch.Elapsed.TotalMilliseconds;
        newRound.PromptTokens = tokenInfo.PromptTokens;
        newRound.CompletionTokens = tokenInfo.CompletionTokens;

        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();

        // Send completion message
        await SendCompletionData(httpResponse, newRound, cancellationToken);
    }

    #region Streaming Helper Methods

    // Simple class to track token usage
    private class TokenInfo
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
    }

    private async Task ProcessStreamingResponse(
        HttpResponse httpResponse,
        List<Message> historyMessages,
        ChatRound newRound,
        StringBuilder responseBuilder,
        TokenInfo tokenInfo,
        CancellationToken cancellationToken)
    {
        var chatRequest = new ChatRequest
        {
            Messages = [.. historyMessages, Message.NewUserMessage(newRound.UserMessage)],
            Temperature = 0.7,
            MaxTokens = 2000,
            Stream = true
        };

        var streamingResponse = _deepSeekClient.ChatStreamAsync(chatRequest, cancellationToken);
        if (streamingResponse == null)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get streaming response from DeepSeek AI");
        }

        // Process each chunk as it arrives
        await foreach (var response in streamingResponse)
        {
            try
            {
                // Extract token usage when available
                if (response.Usage != null)
                {
                    tokenInfo.PromptTokens = response.Usage.PromptTokens;
                    tokenInfo.CompletionTokens = response.Usage.CompletionTokens;
                }

                // Extract and accumulate content
                var choice = response.Choices.FirstOrDefault();
                if (choice?.Delta != null && !string.IsNullOrEmpty(choice.Delta.Content))
                {
                    responseBuilder.Append(choice.Delta.Content);
                }

                // Forward the raw response to client
                await httpResponse.WriteAsync($"data: {JsonSerializer.Serialize(response, _jsonOptions)}\n\n", cancellationToken);
                await httpResponse.Body.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing streaming chunk");
            }
        }

        // Send stream end marker
        await httpResponse.WriteAsync("data: [DONE]\n\n", cancellationToken);
        await httpResponse.Body.FlushAsync(cancellationToken);
    }

    private async Task SendInitialData(HttpResponse httpResponse, ChatRound newRound, CancellationToken cancellationToken)
    {
        var initialData = new
        {
            chatRoundId = newRound.Id,
            sessionId = newRound.SessionId,
            userMessage = newRound.UserMessage,
            model = newRound.Model,
            isActive = newRound.IsActive,
            createdAt = newRound.CreatedAt,
            type = "init"
        };

        await SendSSEMessage(httpResponse, JsonSerializer.Serialize(initialData, _jsonOptions), "init", cancellationToken);
    }

    private async Task SendCompletionData(HttpResponse httpResponse, ChatRound newRound, CancellationToken cancellationToken)
    {
        var finalData = new
        {
            chatRoundId = newRound.Id,
            sessionId = newRound.SessionId,
            userMessage = newRound.UserMessage,
            assistantMessage = newRound.AssistantMessage,
            model = newRound.Model,
            isActive = newRound.IsActive,
            createdAt = newRound.CreatedAt,
            promptTokens = newRound.PromptTokens,
            completionTokens = newRound.CompletionTokens,
            timeTaken = newRound.TimeTaken,
            type = "done"
        };

        await SendSSEMessage(httpResponse, JsonSerializer.Serialize(finalData, _jsonOptions), "done", cancellationToken);
    }

    private async Task HandleStreamingError(
        HttpResponse httpResponse,
        Exception ex,
        ChatRound newRound,
        StringBuilder responseBuilder,
        Stopwatch stopwatch,
        TokenInfo tokenInfo,
        CancellationToken cancellationToken)
    {
        _logger.LogError(ex, "Error streaming response from DeepSeek AI");
        await SendSSEMessage(httpResponse, "Error: " + ex.Message, "error", cancellationToken);

        // Save partial response
        newRound.AssistantMessage = responseBuilder.ToString() + "\n\n[Error: Response was interrupted]";
        newRound.TimeTaken = (int)stopwatch.Elapsed.TotalMilliseconds;
        newRound.PromptTokens = tokenInfo.PromptTokens;
        newRound.CompletionTokens = tokenInfo.CompletionTokens;

        await _chatRoundRepository.CreateAsync(newRound);
        await _chatRoundRepository.SaveChangesAsync();
    }

    private async Task SendSSEMessage(HttpResponse response, string data, string eventType, CancellationToken cancellationToken)
    {
        // Format according to SSE specification
        var message = $"event: {eventType}\ndata: {data}\n\n";
        await response.WriteAsync(message, cancellationToken);
        await response.Body.FlushAsync(cancellationToken);
    }

    #endregion

    #region Helper Methods

    private void ValidateUserMessage(string userMessage)
    {
        if (string.IsNullOrEmpty(userMessage?.Trim()))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "User message is required");
    }

    private async Task<List<Message>> GetHistoryMessages(long sessionId, List<long>? historyChatRoundIds)
    {
        var chatRounds = await _chatRoundRepository.GetChatsBySessionId(sessionId);

        return chatRounds
            .Where(c => c.IsActive)
            .Where(c => historyChatRoundIds == null || historyChatRoundIds.Count == 0 || historyChatRoundIds.Contains(c.Id))
            .SelectMany(c => c.ToMessages())
            .ToList();
    }

    private ChatRound CreateNewChatRound(long sessionId, string userMessage, string? model)
    {
        return new ChatRound
        {
            Id = EntityUtil.GenerateSnowflakeId(),
            SessionId = sessionId,
            UserMessage = userMessage,
            Model = model ?? "",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private async Task<ChatResponse> QueryDeepSeek(List<Message> messages, bool stream = false, CancellationToken cancellationToken = default)
    {
        var chatRequest = new ChatRequest
        {
            Messages = messages,
            Temperature = 0.7,
            MaxTokens = 2000,
            Stream = stream
        };

        // Get AI response
        var response = await _deepSeekClient.ChatAsync(chatRequest, cancellationToken);

        if (response == null)
        {
            _logger.LogError("Failed to get response from DeepSeek AI");
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to get response from DeepSeek AI");
        }

        return response;
    }

    #endregion

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