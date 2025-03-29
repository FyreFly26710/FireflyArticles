using System;
using System.Collections.Concurrent;
using FF.Articles.Backend.AI.Models;

namespace FF.Articles.Backend.AI.API.Services.Stores;

public class UserChatStateStore
{
    // UserId : UserChatState
    private readonly ConcurrentDictionary<long, UserChatState> _userStates = new();

    public UserChatState GetOrAddState(long userId)
    {
        return _userStates.GetOrAdd(userId, _ => new UserChatState());
    }

    public void RemoveState(long userId)
    {
        _userStates.TryRemove(userId, out _);
    }
}

public class UserChatState
{
    public List<Conversation> Conversations { get; set; } = [];
}

public class Conversation
{
    public long ConversationId { get; set; }
    public List<ChatRound> Rounds { get; set; } = [];
    public List<Message?> HistoryMessages
    {
        get => Rounds.SelectMany(r => new[] { r.UserMessage, r.Response.Choices.FirstOrDefault()?.Message })
                    .Where(m => m != null)
                    .ToList();
    }
    public List<Message?> SelectMessages(List<int> roundIds)
    {
        var messages = new List<Message?>();

        foreach (var roundId in roundIds)
        {
            var round = Rounds.FirstOrDefault(r => r.ChatRoundId == roundId);
            if (round != null)
            {
                messages.Add(round.UserMessage);
                messages.Add(round.Response.Choices.FirstOrDefault()?.Message);
            }
        }
        return messages;

    }
}
public class ChatRound
{
    public int ChatRoundId { get; set; }
    public Message UserMessage { get; set; } = new();
    public ChatResponse Response { get; set; } = new();
}

