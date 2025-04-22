using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.AI.API.Models.Requests.Sessions;
namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface ISessionService : IBaseService<Session>
{
    Task<List<SessionDto>> GetSessions(SessionQueryRequest request, HttpRequest httpRequest, CancellationToken cancellationToken);
    Task<SessionDto> GetSession(long id, CancellationToken cancellationToken);
    Task<bool> UpdateSession(SessionEditRequest request, CancellationToken cancellationToken);
}
