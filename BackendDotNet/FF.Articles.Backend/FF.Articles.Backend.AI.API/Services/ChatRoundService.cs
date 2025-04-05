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
namespace FF.Articles.Backend.AI.API.Services;

public class ChatRoundService (
    IChatRoundRepository _chatRoundRepository,
    ISessionRepository _sessionRepository,
    IDeepSeekClient _deepSeekClient,
    ILogger<ChatRoundService> _logger
)
: BaseService<ChatRound, AIDbContext>(_chatRoundRepository, _logger), IChatRoundService
{
    public async Task<ChatRoundDto> NewChatRound(ChatRoundCreateRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        //var user = UserUtil.GetUserFromHttpRequest(httpRequest);

        var newSession = new Session{
            Id = request.SessionId,
            SessionName = request.SessionName,
            UserId = 1
        };
        var sessionId = await _sessionRepository.GetOrCreateSessionId(newSession);
        var chatRounds = await _chatRoundRepository.GetChatsBySessionId(sessionId);
        
        var historyChatRoundIds = request.HistoryChatRoundIds;
        chatRounds = chatRounds.Where(c => historyChatRoundIds == null || historyChatRoundIds.Count == 0 || historyChatRoundIds.Contains(c.Id)).ToList();
        var historyMessages = chatRounds.SelectMany(c => c.ToMessages()).ToList();

        var newRound = request.ToChatRound();
        newRound.SessionId = sessionId;
        // Get AI response
        var stopwatch = Stopwatch.StartNew();
        var response = await queryDeepSeek([..historyMessages, Message.NewUserMessage(newRound.UserMessage)], cancellationToken);
        stopwatch.Stop();

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

    public async Task<ChatRoundDto> ReQueryChatRound(ChatRoundEditRequest request, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        var chatRound = await _chatRoundRepository.GetByIdAsync(request.ChatRoundId);
        if (chatRound == null)
        {
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Chat round not found");
        }
        List<Message> messages = new List<Message>();
        if (request.IncludeHistory){
            var chatRounds = await _chatRoundRepository.GetChatsBySessionId(request.SessionId);
            chatRounds = chatRounds.Where(c => c.Id == request.ChatRoundId).ToList();
            messages = chatRounds.SelectMany(c => c.ToMessages()).ToList();
        }
        // Get AI response
        var stopwatch = Stopwatch.StartNew();
        var response = await queryDeepSeek([..messages, Message.NewUserMessage(request.UserMessage)], cancellationToken);
        stopwatch.Stop();

        // Store the response in the chat round
        chatRound.AssistantMessage = response.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
        chatRound.PromptTokens = response?.Usage?.PromptTokens ?? 0;
        chatRound.CompletionTokens = response?.Usage?.CompletionTokens ?? 0;
        chatRound.TimeTaken = (int)stopwatch.Elapsed.TotalMilliseconds;

        // Add the chat round to the conversation
        await _chatRoundRepository.UpdateAsync(chatRound);
        await _chatRoundRepository.SaveChangesAsync();

        return chatRound.ToDto();
    }

    private async Task<ChatResponse> queryDeepSeek(List<Message> messages, CancellationToken cancellationToken = default)
    {

        var chatRequest = new ChatRequest
        {
            Messages = messages,
            Temperature = 0.7,
            MaxTokens = 2000,
            Stream = false
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
}