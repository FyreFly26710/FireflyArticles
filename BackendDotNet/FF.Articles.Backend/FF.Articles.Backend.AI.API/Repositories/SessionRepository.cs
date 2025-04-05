using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;
using FF.Articles.Backend.Common.Utils;
using Microsoft.EntityFrameworkCore;
namespace FF.Articles.Backend.AI.API.Repositories;

public class SessionRepository : BaseRepository<Session, AIDbContext>, ISessionRepository
{
    public SessionRepository(AIDbContext context) : base(context)
    {
    }

    public async Task<long> GetOrCreateSessionId(Session session)
    {
        var existingSession = await GetByIdAsync(session.Id);
        if (existingSession == null)
        {
            session.Id = EntityUtil.GenerateSnowflakeId();
            await this.CreateAsync(session);
            await SaveChangesAsync();
            return session.Id;
        }
        return existingSession.Id;
    }
    public async Task<List<Session>> GetSessionsByUserId(long userId)
    {
        return await GetQueryable().Where(s => s.UserId == userId).ToListAsync();
    }
}
