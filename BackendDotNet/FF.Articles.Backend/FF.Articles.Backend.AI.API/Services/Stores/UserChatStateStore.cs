using System;
using System.Collections.Concurrent;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;

namespace FF.Articles.Backend.AI.API.Services.Stores;

public class UserChatStateStore
{
    public List<Session> Sessions { get; set; } = [];
    // public Session GetOrCreateSession(long userId, long sessionId)
    // {
    //     var session = Sessions.FirstOrDefault(s => s.SessionId == sessionId);
    //     if (session == null)
    //     {
    //         session = new Session { SessionId = sessionId, UserId = userId };
    //         Sessions.Add(session);
    //     }
    //     return session;
    // }
    // public List<Session> GetSessionsByUserId(long userId)
    // {
    //     return Sessions.Where(s => s.UserId == userId).ToList();
    // }
    // public void DeleteSession(long sessionId)
    // {
    //     var session = Sessions.FirstOrDefault(s => s.SessionId == sessionId);
    //     if (session != null)
    //     {
    //         Sessions.Remove(session);
    //     }
    // }
    // public void DeleteRound(long sessionId, long roundId)
    // {
    //     var session = Sessions.FirstOrDefault(s => s.SessionId == sessionId);
    //     if (session != null)
    //     {
    //         session.Rounds.RemoveAll(r => r.ChatRoundId == roundId);
    //     }
    // }
    // public Session? GetSessionById(long sessionId)
    // {
    //     return Sessions.FirstOrDefault(s => s.SessionId == sessionId);
    // }
}



