using System;

namespace FF.Articles.Backend.AI.API.Models.Dtos;

public class ChatRoundDto
{
    public long SessionId { get; set; }
    public long ChatRoundId { get; set; }
    public string UserMessage { get; set; } = string.Empty;
    public string AssistantMessage { get; set; } = string.Empty; 
    public string Model { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens => PromptTokens + CompletionTokens;
    public int TimeTaken { get; set; }
}