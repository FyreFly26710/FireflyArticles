using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.Chats;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/sessions")]
public class SessionController(ISessionService sessionService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<SessionDto>> GetSession(long id, CancellationToken cancellationToken)
    {
        var result = await sessionService.GetSession(id, cancellationToken);
        return ResultUtil.Success(result);
    }
    [HttpGet]
    public async Task<ApiResponse<List<SessionDto>>> GetSessions(SessionQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await sessionService.GetSessions(request, HttpContext.Request, cancellationToken);
        return ResultUtil.Success(result);
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteSession(long id, CancellationToken cancellationToken)
    {
        var result = await sessionService.DeleteAsync(id);
        return ResultUtil.Success(result);
    }
}
