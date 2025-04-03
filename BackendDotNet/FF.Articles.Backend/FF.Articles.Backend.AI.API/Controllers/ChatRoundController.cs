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
[Route("api/ai/chat-rounds")]
public class ChatRoundController(IChatRoundService chatRoundService) : ControllerBase
{

    [HttpPost]
    public async Task<ApiResponse<ChatRoundDto>> NewChatByRequest([FromBody] ChatRoundCreateRequest request, CancellationToken cancellationToken)
    {
        // If streaming is enabled, use the streaming endpoint instead
        if (request.EnableStreaming)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "For streaming responses, use the /stream endpoint");
        }

        var result = await chatRoundService.NewChatRound(request, HttpContext.Request, cancellationToken);
        return ResultUtil.Success(result);
    }

    [HttpPost("stream")]
    public async Task StreamChatResponse([FromBody] ChatRoundCreateRequest request, CancellationToken cancellationToken)
    {
        // Set content type for SSE
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        // Force streaming to be true
        request.EnableStreaming = true;

        // Use the service to stream the response
        await chatRoundService.StreamChatRound(request, HttpContext.Request, Response, cancellationToken);
    }
    // [HttpPut]
    // public async Task<ApiResponse<ChatRoundDto>> UpdateByRequest([FromBody] ChatRoundReQueryRequest request, CancellationToken cancellationToken)
    // {
    //     await checkUserPermission(new List<long> { request.ChatRoundId });
    //     var result = await chatRoundService.ReQueryChatRound(request, HttpContext.Request, cancellationToken);
    //     return ResultUtil.Success(result);
    // }
    [HttpPut("enable")]
    public async Task<ApiResponse<bool>> Enable([FromBody] List<long> ids)
    {
        await checkUserPermission(ids);
        var result = await chatRoundService.EnableChatRound(ids);
        return ResultUtil.Success(result);
    }
    [HttpPut("disable")]
    public async Task<ApiResponse<bool>> Disable([FromBody] List<long> ids)
    {
        await checkUserPermission(ids);
        var result = await chatRoundService.DisableChatRound(ids);
        return ResultUtil.Success(result);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteById(long id)
    {
        await checkUserPermission(new List<long> { id });
        var result = await chatRoundService.DeleteAsync(id);
        return ResultUtil.Success(result);
    }
    [HttpDelete]
    public async Task<ApiResponse<bool>> DeleteByIds([FromBody] List<long> ids)
    {
        await checkUserPermission(ids);
        var result = await chatRoundService.DeleteAsync(ids);
        return ResultUtil.Success(result);
    }

    private async Task checkUserPermission(List<long> ids)
    {
        var userId = await chatRoundService.GetUserIdByChatRoundId(ids);
        var currentUserId = UserUtil.GetUserId(HttpContext.Request);
        if (userId != currentUserId)
            throw new ApiException(ErrorCode.FORBIDDEN_ERROR, "You are not allowed to enable this chat round");
    }

}
