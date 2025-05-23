namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/chat-rounds")]
public class ChatRoundController(IChatRoundService chatRoundService) : ControllerBase
{

    [HttpPost]
    public async Task<ApiResponse<ChatRoundDto>> NewChatByRequest([FromBody] ChatRoundCreateRequest request, CancellationToken cancellationToken)
    {
        if (request.Provider == ProviderList.Gemini)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Gemini is not supported yet");
        }
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var result = await chatRoundService.NewChatRound(request, user, cancellationToken);
        return ResultUtil.Success(result);
    }

    [HttpPost("stream")]
    public async Task StreamChat([FromBody] ChatRoundCreateRequest request, CancellationToken cancellationToken)
    {
        if (request.Provider == ProviderList.Gemini)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Gemini is not supported yet");
        }

        var user = UserUtil.GetUserFromHttpRequest(Request);

        // Set content type for SSE
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        try
        {
            // Use the service to stream the response
            await foreach (var sseDto in chatRoundService.StreamChatRound(request, user, cancellationToken))
            {
                // Format according to SSE specification: event: <event>\ndata: <data>\n\n
                var message = $"event: {sseDto.Event}\ndata: {sseDto.Data}\n\n";
                await Response.WriteAsync(message, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"event: {SseEvent.Error}\ndata: {JsonSerializer.Serialize(new { message = ex.Message })}\n\n";
            await Response.WriteAsync(errorMessage, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            throw;
        }
        finally
        {
            await Response.CompleteAsync();
        }
    }

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
