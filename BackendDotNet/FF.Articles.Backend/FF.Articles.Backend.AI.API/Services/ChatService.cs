using System;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Services.Stores;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;
using FF.Articles.Backend.AI.Services;
using FF.Articles.Backend.Common.Utils;
using Microsoft.Extensions.Logging;

namespace FF.Articles.Backend.AI.API.Services;

public class ChatService : IChatService
{
    private readonly IDeepSeekClient _deepSeekClient;
    private readonly IContentsApiRemoteService _contentsApiRemoteService;
    private readonly UserChatStateStore _userChatStateStore;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IDeepSeekClient deepSeekClient,
        IContentsApiRemoteService contentsApiRemoteService,
        UserChatStateStore userChatStateStore,
        ILogger<ChatService> logger)
    {
        _deepSeekClient = deepSeekClient;
        _contentsApiRemoteService = contentsApiRemoteService;
        _userChatStateStore = userChatStateStore;
        _logger = logger;
    }

    public async Task<string> ChatAsync(string message, long conversationId, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get or create user state
            var user = UserUtil.GetUserFromHttpRequest(httpRequest);
            var userState = _userChatStateStore.GetOrAddState(user.UserId);

            // Get or create conversation
            var conversation = userState.Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conversation == null)
            {
                conversation = new Conversation { ConversationId = conversationId };
                userState.Conversations.Add(conversation);
            }
            var userMessage = Message.NewUserMessage(message);
            // Create new chat round
            var chatRound = new ChatRound
            {
                ChatRoundId = conversation.Rounds.Count + 1,
                UserMessage = userMessage
            };
            // Create chat request with conversation history
            var chatRequest = new ChatRequest
            {
                Messages = [..conversation.HistoryMessages, userMessage],
                Temperature = 0.7,
                MaxTokens = 2000,
                Stream = false
            };

            // Get AI response
            var response = await _deepSeekClient.ChatAsync(chatRequest, cancellationToken);

            if (response == null)
            {
                _logger.LogError("Failed to get response from DeepSeek AI");
                return "I apologize, but I'm having trouble processing your request at the moment. Please try again later.";
            }

            // Store the response in the chat round
            chatRound.Response = response;

            // Add the chat round to the conversation
            conversation.Rounds.Add(chatRound);

            // Extract and return AI's response
            var aiMessage = response.Choices.FirstOrDefault()?.Message;
            if (aiMessage == null)
            {
                _logger.LogError("No message in AI response");
                return "I apologize, but I couldn't generate a proper response. Please try again.";
            }

            return aiMessage.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChatAsync for conversation {ConversationId}", conversationId);
            return "I apologize, but an error occurred while processing your request. Please try again later.";
        }
    }
}
