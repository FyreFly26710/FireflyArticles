using System;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/chat")]
public class AiChatController(IChatService chatService) : ControllerBase
{
    [HttpPost("get-message")]
    public async Task<IActionResult> Chat([FromBody] ClientMessagesRequest request, CancellationToken cancellationToken)
    {
        //var result = await chatService.ChatAsync(request.Message, request.ConversationId, HttpContext.Request, cancellationToken);
        var result = request.Message;
        return Ok(result);
    }
}
