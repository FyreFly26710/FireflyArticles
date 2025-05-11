namespace FF.Articles.Backend.AI.API.Interfaces.Repositories;

public interface IChatRoundRepository : IBaseRepository<ChatRound, AIDbContext>
{
    Task<List<ChatRound>> GetChatsBySessionId(long sessionId);
    Task<List<ChatRound>> GetChatsBySessionIds(List<long> sessionIds);
}
