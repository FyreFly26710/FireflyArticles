using System;
using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.AI.API.Models.Entities;

public class ChatRound : BaseEntity
{
    public long SessionId { get; set; }
    public long? SystemMessageId { get; set; }
    public string UserMessage { get; set; } = string.Empty;
    public string AssistantMessage { get; set; } = string.Empty; 
    public string Model { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int TimeTaken { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    //public int TotalTokens => PromptTokens + CompletionTokens;
    // public List<Message> ToMessages()
    // {
    //     return new List<Message> {
    //         new Message { Content = UserMessage, Role = MessageRoles.User },
    //         new Message { Content = AssistantMessage, Role = MessageRoles.Assistant }
    //     };
    // }
}