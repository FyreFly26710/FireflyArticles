using System;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Requests.Chats;
using FF.Articles.Backend.AI.API.Services.Stores;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;

namespace FF.Articles.Backend.AI.API.MapperExtensions.Chats;

public static class ChatMessageRequestExtensions
{
    public static ChatRound ToChatRound(this ChatRoundCreateRequest request)
    {
        return new ChatRound
        {            
            SessionId = request.SessionId,
            UserMessage = request.UserMessage,
            Model = request.Model,
            CreatedAt = DateTime.UtcNow
        };
    }
    public static ChatRoundDto ToDto(this ChatRound chatRound)
    {
        return new ChatRoundDto
        {
            ChatRoundId = chatRound.Id,
            SessionId = chatRound.SessionId,
            UserMessage = chatRound.UserMessage,
            AssistantMessage = chatRound.AssistantMessage,
            Model = chatRound.Model,
            CreatedAt = chatRound.CreatedAt,
            PromptTokens = chatRound.PromptTokens,
            CompletionTokens = chatRound.CompletionTokens,
            TimeTaken = chatRound.TimeTaken
        };
    }

    public static List<Message> ToMessages(this ChatRound chatRound)
    {
        return new List<Message> {
            new Message { Content = chatRound.UserMessage, Role = MessageRoles.User },
            new Message { Content = chatRound.AssistantMessage, Role = MessageRoles.Assistant }
        };
    }
    public static SessionDto ToDto(this Session session)
    {
        return new SessionDto
        {
            SessionId = session.Id,
            SessionName = session.SessionName,
        };
    }
}
