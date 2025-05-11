namespace FF.Articles.Backend.AI.API.Controllers;

[Route("api/ai/assistants")]
[ApiController]
public class AssistantController(IAssistant _aiChatAssistant) : ControllerBase
{
    [HttpGet("providers")]
    public async Task<ApiResponse<List<ChatProvider>>> GetProviders()
    {
        var response = await _aiChatAssistant.GetProviderAsync(CancellationToken.None);
        return ResultUtil.Success(response);
    }
}
