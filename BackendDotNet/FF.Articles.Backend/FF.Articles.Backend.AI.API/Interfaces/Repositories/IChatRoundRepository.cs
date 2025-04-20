using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Models.Entities;

namespace FF.Articles.Backend.AI.API.Interfaces.Repositories;

public interface IChatRoundRepository : IBaseRepository<ChatRound, AIDbContext>
{
    Task<List<ChatRound>> GetChatsBySessionId(long sessionId);
    Task<List<ChatRound>> GetChatsBySessionIds(List<long> sessionIds);
}
