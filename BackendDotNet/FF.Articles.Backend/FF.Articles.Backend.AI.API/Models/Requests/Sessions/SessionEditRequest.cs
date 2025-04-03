using System;

namespace FF.Articles.Backend.AI.API.Models.Requests.Chats;

public class SessionEditRequest
{
    public long SessionId { get; set; }
    public string? SessionName { get; set; }

}
