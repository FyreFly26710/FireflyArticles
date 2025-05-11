namespace FF.Articles.Backend.AI.API.Interfaces.Repositories;

public interface ISessionRepository : IBaseRepository<Session, AIDbContext>
{
    Task<List<Session>> GetSessionsByUserId(long userId);
}
