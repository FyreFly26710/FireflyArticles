namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface ISessionService : IBaseService<Session>
{
    Task<List<SessionDto>> GetSessions(SessionQueryRequest request, long userId, CancellationToken cancellationToken);
    Task<SessionDto> GetSession(long id, long userId, CancellationToken cancellationToken);
    Task<bool> UpdateSession(SessionEditRequest request, UserApiDto user);
    Task<bool> DeleteSession(long id, UserApiDto user);
}
