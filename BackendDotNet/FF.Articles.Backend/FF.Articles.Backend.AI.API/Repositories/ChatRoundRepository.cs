namespace FF.Articles.Backend.AI.API.Repositories;

public class ChatRoundRepository : BaseRepository<ChatRound, AIDbContext>, IChatRoundRepository
{
    public ChatRoundRepository(AIDbContext context) : base(context)
    {
    }

    public async Task<List<ChatRound>> GetChatsBySessionId(long sessionId)
    {
        var query = GetQueryable().Where(c => c.SessionId == sessionId).OrderBy(c => c.Id);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<ChatRound>> GetChatsBySessionIds(List<long> sessionIds)
    {
        var query = GetQueryable().Where(c => sessionIds.Contains(c.SessionId)).OrderBy(c => c.Id);
        var result = await query.ToListAsync();
        return result;
    }
}
