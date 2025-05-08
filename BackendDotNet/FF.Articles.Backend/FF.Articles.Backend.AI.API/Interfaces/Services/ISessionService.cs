using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.AI.API.Models.Requests.Sessions;
using FF.Articles.Backend.Common.ApiDtos;
namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface ISessionService : IBaseService<Session>
{
    Task<List<SessionDto>> GetSessions(SessionQueryRequest request, long userId, CancellationToken cancellationToken);
    Task<SessionDto> GetSession(long id, long userId, CancellationToken cancellationToken);
    Task<bool> UpdateSession(SessionEditRequest request, UserApiDto user);
    Task<bool> DeleteSession(long id, UserApiDto user);
}
