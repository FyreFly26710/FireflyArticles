using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.Chats;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/chat-rounds")]
public class ChatRoundController(IChatRoundService chatRoundService) : ControllerBase
{
    [HttpPost]
    public async Task<ApiResponse<ChatRoundDto>> AddByRequest([FromBody] ChatRoundCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await chatRoundService.NewChatRound(request, HttpContext.Request, cancellationToken);
        return ResultUtil.Success(result);
    }
    [HttpPut]
    public async Task<ApiResponse<ChatRoundDto>> UpdateByRequest([FromBody] ChatRoundEditRequest request, CancellationToken cancellationToken)
    {
        var result = await chatRoundService.ReQueryChatRound(request, HttpContext.Request, cancellationToken);
        return ResultUtil.Success(result);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteById(long id, CancellationToken cancellationToken)
    {
        var result = await chatRoundService.DeleteAsync(id);
        return ResultUtil.Success(result);
    }

}
