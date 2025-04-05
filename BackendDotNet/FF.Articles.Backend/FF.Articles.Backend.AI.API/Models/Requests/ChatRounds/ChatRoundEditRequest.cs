using System;

namespace FF.Articles.Backend.AI.API.Models.Requests.Chats;

public class ChatRoundEditRequest
{
    public long ChatRoundId { get; set; }
    public long SessionId { get; set; }
    public string? UserMessage { get; set; }
    public string? Model { get; set; }
    public bool IncludeHistory { get; set; } = false;
}
