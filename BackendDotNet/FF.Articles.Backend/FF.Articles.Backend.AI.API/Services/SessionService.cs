namespace FF.Articles.Backend.AI.API.Services;

public class SessionService(
    ISessionRepository _sessionRepository,
    IChatRoundRepository _chatRoundRepository,
    ILogger<SessionService> _logger
)
: BaseService<Session, AIDbContext>(_sessionRepository, _logger), ISessionService
{
    public async Task<List<SessionDto>> GetSessions(SessionQueryRequest request, long userId, CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetSessionsByUserId(userId);
        if (sessions == null || sessions.Count == 0) return [];

        var chatRounds = await _chatRoundRepository.GetChatsBySessionIds(sessions.Select(s => s.Id).ToList());
        var sessionDtos = sessions
            .Select(s => getSessionDto(s, chatRounds.Where(c => c.SessionId == s.Id).ToList(), request.IncludeChatRounds))
            .OrderByDescending(s => s.SessionId)
            .ToList();
        return sessionDtos;
    }

    public async Task<SessionDto> GetSession(long id, long userId, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Session not found");
        if (session.UserId != userId)
            throw new ApiException(ErrorCode.FORBIDDEN_ERROR, "You are not allowed to access this session");
        var chatRounds = await _chatRoundRepository.GetChatsBySessionId(id);
        return getSessionDto(session, chatRounds, true);
    }

    private SessionDto getSessionDto(Session session, List<ChatRound> chatRounds, bool includeChatRounds = false) =>
    new()
    {
        SessionId = session.Id,
        SessionName = session.SessionName ?? chatRounds.LastOrDefault()?.UserMessage ?? "New Chat",
        RoundCount = chatRounds.Count,
        Rounds = includeChatRounds ? chatRounds.Select(c => c.ToDto()).ToList() : [],
        TimeStamp = session.TimeStamp,
        CreateTime = session.CreateTime ?? DateTime.Now,
        UpdateTime = session.UpdateTime ?? DateTime.Now
    };

    public async Task<bool> UpdateSession(SessionEditRequest request, UserApiDto user)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);
        if (session == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Session not found");
        if (!string.IsNullOrEmpty(request?.SessionName?.Trim()))
            session.SessionName = request.SessionName;
        await _sessionRepository.UpdateAsync(session);
        await _sessionRepository.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteSession(long id, UserApiDto user)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Session not found");
        if (session.UserId != user.UserId)
            throw new ApiException(ErrorCode.FORBIDDEN_ERROR, "You are not allowed to delete this session");
        await _sessionRepository.DeleteAsync(id);
        await _sessionRepository.SaveChangesAsync();
        return true;
    }
}
