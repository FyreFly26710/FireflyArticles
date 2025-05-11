namespace FF.Articles.Backend.AI.API.Repositories;

public class SessionRepository : BaseRepository<Session, AIDbContext>, ISessionRepository
{
    public SessionRepository(AIDbContext context) : base(context)
    {
    }
    public async Task<List<Session>> GetSessionsByUserId(long userId)
    {
        var query = GetQueryable().Where(s => s.UserId == userId).OrderBy(s => s.Id);
        var result = await query.ToListAsync();
        return result;
    }
}
