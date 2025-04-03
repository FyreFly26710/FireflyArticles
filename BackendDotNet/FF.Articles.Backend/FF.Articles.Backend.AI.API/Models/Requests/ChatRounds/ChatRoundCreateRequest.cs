using System;

namespace FF.Articles.Backend.AI.API.Models.Requests.Chats;

public class ChatRoundCreateRequest
{
    public long SessionId { get; set; }
    public List<long>? HistoryChatRoundIds { get; set; }
    public string UserMessage { get; set; } = "";
    public string? Model { get; set; } = null;
    public bool EnableStreaming { get; set; } = false;
}