using System;
using System.Collections.Concurrent;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.AI.Models;
using FF.Articles.Backend.Common.ApiDtos;

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
    public List<Message> HistroyMessages { get; set; } = new();
    public ArticlesAIResponseDto? ArticlesAIResponse { get; set; }
    public string Topic { get; set; } = "";
    public bool IsFirstRound { get; set; } = true;
    public List<ArticleApiAddRequest> ApiAddRequests { get; set; } = new();
}