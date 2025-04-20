using System;

namespace FF.Articles.Backend.AI.API.Models.Requests.Chats;

public class SessionQueryRequest
{
    public bool IncludeChatRounds { get; set; } = false;
}
