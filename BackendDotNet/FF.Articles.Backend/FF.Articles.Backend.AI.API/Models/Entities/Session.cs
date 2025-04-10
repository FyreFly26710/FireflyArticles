using System;
using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.AI.API.Models.Entities;

public class Session : BaseEntity
{
    public long UserId { get; set; }
    public string? SessionName { get; set; }
    public long TimeStamp { get; set; }

}
