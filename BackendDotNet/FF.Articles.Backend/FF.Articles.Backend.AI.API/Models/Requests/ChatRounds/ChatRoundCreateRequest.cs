using System;

namespace FF.Articles.Backend.AI.API.Models.Requests.Chats;

public class ChatRoundCreateRequest
{
    // If sessionId is not provided, the session will be created
    public long? SessionId { get; set; }
    public long SessionTimeStamp { get; set; }
    public List<long>? HistoryChatRoundIds { get; set; }
    public string UserMessage { get; set; } = "";
    public string? Model { get; set; } = null;
}