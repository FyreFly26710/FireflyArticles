using System;

namespace FF.Articles.Backend.AI.API.Models.Dtos;

public class SessionDto
{
    public long SessionId { get; set; }
    public string SessionName { get; set; } = "New Session";
    public List<ChatRoundDto> Rounds { get; set; } = [];
    public int RoundCount {get; set;}
}
