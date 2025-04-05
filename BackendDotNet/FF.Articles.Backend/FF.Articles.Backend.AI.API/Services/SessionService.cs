using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.Chats;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.Services;
using FF.Articles.Backend.AI.Models;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.AI.API.MapperExtensions.Chats;
using FF.Articles.Backend.Common.Exceptions;
namespace FF.Articles.Backend.AI.API.Services;

public class SessionService(
    ISessionRepository _sessionRepository,
    IChatRoundRepository _chatRoundRepository,
    ILogger<SessionService> _logger
)
: BaseService<Session, AIDbContext>(_sessionRepository, _logger), ISessionService
{
    public async Task<List<SessionDto>> GetSessions(SessionQueryRequest request, HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        //var user = UserUtil.GetUserFromHttpRequest(httpRequest);
        var sessions = await _sessionRepository.GetSessionsByUserId(1);

        var sessionDtos = sessions.Select(s => s.ToDto()).ToList();
        var chatRounds = await _chatRoundRepository.GetChatsBySessionIds(sessionDtos.Select(s => s.SessionId).ToList());
        foreach (var sessionDto in sessionDtos)
        {
            var rounds = chatRounds.Where(c => c.SessionId == sessionDto.SessionId).Select(c => c.ToDto()).ToList();
            sessionDto.RoundCount = rounds.Count;
            if (request.IncludeChatRounds) sessionDto.Rounds = rounds;
        }
        return sessionDtos;
    }

    public async Task<SessionDto> GetSession(long id, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
        {
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Session not found");
        }
        var chatRounds = await _chatRoundRepository.GetChatsBySessionId(id);
        var sessionDto = session.ToDto();
        sessionDto.Rounds = chatRounds.Select(c => c.ToDto()).ToList();
        sessionDto.RoundCount = chatRounds.Count;
        return sessionDto;
    }
}
