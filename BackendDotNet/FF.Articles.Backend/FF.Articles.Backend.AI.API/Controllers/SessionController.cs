using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.Sessions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/sessions")]
public class SessionController(ISessionService sessionService) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ApiResponse<SessionDto>> GetSession(long id, CancellationToken cancellationToken)
    {
        var userId = UserUtil.GetUserId(Request);
        var result = await sessionService.GetSession(id, userId, cancellationToken);
        return ResultUtil.Success(result);
    }
    [HttpGet]
    [Authorize]
    public async Task<ApiResponse<List<SessionDto>>> GetSessions([FromQuery] bool includeChatRounds = false, CancellationToken cancellationToken = default)
    {
        var userId = UserUtil.GetUserId(Request);
        var request = new SessionQueryRequest { IncludeChatRounds = includeChatRounds };
        var result = await sessionService.GetSessions(request, userId, cancellationToken);
        return ResultUtil.Success(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<ApiResponse<bool>> UpdateSession(SessionEditRequest request)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var result = await sessionService.UpdateSession(request, user);
        return ResultUtil.Success(result);
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ApiResponse<bool>> DeleteSession(long id)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var result = await sessionService.DeleteSession(id, user);
        return ResultUtil.Success(result);
    }
}
