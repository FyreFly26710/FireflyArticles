using System;

namespace FF.Articles.Backend.AI.API.Models.Requests;

public class ClientMessagesRequest
{
    public string Message { get; set; } = "";
    public long ConversationId { get; set; } = 0;
}