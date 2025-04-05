using System;
using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.AI.API.Models.Entities;

public class Session : BaseEntity
{
    public long UserId { get; set; }
    public string SessionName { get; set; } = "New Session";
    // public List<ChatRound> Rounds { get; set; } = [];
    // public List<Message> HistoryMessages
    // {
    //     get => Rounds.Where(r => string.IsNullOrEmpty(r.AssistantMessage))
    //     .SelectMany(r => r.ToMessages())
    //     .ToList();
    // }
    // public List<Message> SelectMessages(List<long> roundIds)
    // {
    //     var messages = new List<Message>();

    //     foreach (var roundId in roundIds)
    //     {
    //         var round = Rounds.FirstOrDefault(r => r.ChatRoundId == roundId);
    //         if (round != null)
    //         {
    //             messages.AddRange(round.ToMessages());
    //         }
    //     }
    //     return messages;

    // }
}
