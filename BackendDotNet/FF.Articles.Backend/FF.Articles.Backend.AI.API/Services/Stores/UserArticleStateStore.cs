using System;
using System.Collections.Concurrent;
using FF.Articles.Backend.AI.API.Models.AiDtos;

namespace FF.Articles.Backend.AI.API.Services.Stores;
public class UserArticleStateStore
{
    private readonly ConcurrentDictionary<long, UserArticleState> _userStates = new();

    public UserArticleState GetOrAddState(long userId)
    {
        return _userStates.GetOrAdd(userId, _ => new UserArticleState());
    }

    public void RemoveState(long userId)
    {
        _userStates.TryRemove(userId, out _);
    }
}

public class UserArticleState
{
    public ArticlesAIResponse? ArticlesAIResponse { get; set; }
    // AI generated article id (sort number) => Db article id
    public Dictionary<int, long> IdMap { get; set; } = new();
    public string Topic { get; set; } = "";
    public long TopicId { get; set; } = 0;
    public bool IsFirstRound { get; set; } = true;
}