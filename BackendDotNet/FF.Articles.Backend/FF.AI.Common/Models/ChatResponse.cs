using System;
using FF.AI.Common.Constants;

namespace FF.AI.Common.Models;


public class ChatResponse
{
    public Message? Message { get; set; }
    public string? Event { get; set; }
    public ExtraInfo? ExtraInfo { get; set; }

    public ChatResponse()
    {
        Message = new Message();
        Event = ChatEvent.Finish;
        ExtraInfo = new ExtraInfo();
    }
    public ChatResponse(Message message, string @event, ExtraInfo extraInfo)
    {
        Message = message;
        Event = @event;
        ExtraInfo = extraInfo;
    }
}

public class ExtraInfo
{
    public DateTime? CreatedAt { get; set; }
    public string? Model { get; set; }
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }
    /// <summary>
    /// Time taken to generate the response in milliseconds
    /// </summary>
    public int? Duration { get; set; }
}


