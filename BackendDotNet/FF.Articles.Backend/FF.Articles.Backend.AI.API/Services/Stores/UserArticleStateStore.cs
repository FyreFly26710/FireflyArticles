using System;
using System.Collections.Concurrent;
using FF.Articles.Backend.AI.API.Models.AiDtos;

namespace FF.Articles.Backend.AI.API.Services.Stores;
public class UserArticleStateStore
{
    private readonly ConcurrentDictionary<int, UserArticleState> _userStates = new();

    public UserArticleState GetOrAddState(int userId)
    {
        return _userStates.GetOrAdd(userId, _ => new UserArticleState());
    }

    public void RemoveState(int userId)
    {
        _userStates.TryRemove(userId, out _);
    }
}

public class UserArticleState
{
    public ArticlesAIResponse? ArticlesAIResponse { get; set; }
    // AI generated article id (sort number) => Db article id
    public Dictionary<int, int> IdMap { get; set; } = new();
    public string Topic { get; set; } = "";
    public bool IsFirstRound { get; set; } = true;
}