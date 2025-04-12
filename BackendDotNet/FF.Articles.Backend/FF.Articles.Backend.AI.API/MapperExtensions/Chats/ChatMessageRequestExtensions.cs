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

    public static ChatRoundDto ToDto(this ChatRound chatRound)
    {
        return new ChatRoundDto
        {
            ChatRoundId = chatRound.Id,
            SessionId = chatRound.SessionId,
            UserMessage = chatRound.UserMessage,
            AssistantMessage = chatRound.AssistantMessage,
            Model = chatRound.Model,
            PromptTokens = chatRound.PromptTokens,
            CompletionTokens = chatRound.CompletionTokens,
            TimeTaken = chatRound.TimeTaken,
            IsActive = chatRound.IsActive,
            CreateTime = chatRound.CreateTime ?? DateTime.Now,
            UpdateTime = chatRound.UpdateTime ?? DateTime.Now
        };
    }

    public static List<Message> ToMessages(this ChatRound chatRound)
    {
        return new List<Message> {
            new() { Content = chatRound.UserMessage, Role = MessageRoles.User },
            new() { Content = chatRound.AssistantMessage, Role = MessageRoles.Assistant }
        };
    }
}
