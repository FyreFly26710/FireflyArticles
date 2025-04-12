using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.AI.API.Repositories;

public class ChatRoundRepository : BaseRepository<ChatRound, AIDbContext>, IChatRoundRepository
{
    public ChatRoundRepository(AIDbContext context) : base(context)
    {
    }

    public async Task<List<ChatRound>> GetChatsBySessionId(long sessionId)
    {
        return await GetQueryable().Where(c => c.SessionId == sessionId).OrderBy(c=>c.Id).ToListAsync();
    }

    public async Task<List<ChatRound>> GetChatsBySessionIds(List<long> sessionIds)
    {
        return await GetQueryable().Where(c => sessionIds.Contains(c.SessionId)).OrderBy(c=>c.Id).ToListAsync();
    }
}
