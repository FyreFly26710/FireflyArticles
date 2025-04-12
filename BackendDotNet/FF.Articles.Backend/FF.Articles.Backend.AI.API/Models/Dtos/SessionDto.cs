using System;

namespace FF.Articles.Backend.AI.API.Models.Dtos;

public class SessionDto
{
    public long SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public List<ChatRoundDto> Rounds { get; set; } = [];
    public int RoundCount { get; set; }
    public long TimeStamp { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
