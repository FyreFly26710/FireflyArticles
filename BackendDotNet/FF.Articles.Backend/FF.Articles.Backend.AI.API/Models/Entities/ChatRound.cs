namespace FF.Articles.Backend.AI.API.Models.Entities;

public class ChatRound : BaseEntity
{
    public long SessionId { get; set; }
    public long? SystemMessageId { get; set; }
    public string UserMessage { get; set; } = string.Empty;
    public string AssistantMessage { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public long TimeStamp { get; set; }
    public int TimeTaken { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public bool IsActive { get; set; } = true;
}