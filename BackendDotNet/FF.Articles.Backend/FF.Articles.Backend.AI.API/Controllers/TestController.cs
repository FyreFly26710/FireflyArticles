using System;
using Microsoft.AspNetCore.Mvc;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Providers;
using FF.AI.Common.Models;
using System.Text.Json;
namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/test")]
public class TestController : ControllerBase
{
    private readonly IAssistant<DeepSeekProvider> _deepSeekAssistant;
    private readonly IAssistant<OllamaProvider> _ollamaAssistant;
    private readonly IAssistant _aiChatAssistant;
    public TestController(IAssistant<DeepSeekProvider> deepSeekAssistant, IAssistant<OllamaProvider> ollamaAssistant, IAssistant aiChatAssistant)
    {
        _deepSeekAssistant = deepSeekAssistant;
        _ollamaAssistant = ollamaAssistant;
        _aiChatAssistant = aiChatAssistant;
    }
    [HttpPost("deepseek")]
    public async Task<IActionResult> GetDeepSeek([FromBody] ChatRequest request)
    {
        var response = await _aiChatAssistant.ChatAsync(request, CancellationToken.None);
        return Ok(response);
    }
    [HttpPost("ollama")]
    public async Task<IActionResult> GetOllama([FromBody] ChatRequest request)
    {
        var response = await _aiChatAssistant.ChatAsync(request, CancellationToken.None);
        return Ok(response);
    }
    [HttpPost("ollama-stream")]
    public async Task GetOllamaStream([FromBody] ChatRequest request)
    {
        var response = _ollamaAssistant.ChatStreamAsync(request, CancellationToken.None);
        try
        {
            // Use the service to stream the response
            await foreach (var sseDto in response)
            {
                // Format according to SSE specification: event: <event>\ndata: <data>\n\n
                var message = $"event: sseDto.Event\ndata: {JsonSerializer.Serialize(sseDto)}\n\n";
                await Response.WriteAsync(message);
                await Response.Body.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"event: SseEvent.Error\ndata: {JsonSerializer.Serialize(new { message = ex.Message })}\n\n";
            await Response.WriteAsync(errorMessage);
            await Response.Body.FlushAsync();
            throw ex;
        }
        finally
        {
            await Response.CompleteAsync();
        }
    }
    [HttpPost("deepseek-stream")]
    public async Task GetDeepSeekStream([FromBody] ChatRequest request)
    {
        var response = _deepSeekAssistant.ChatStreamAsync(request, CancellationToken.None);
        try
        {
            // Use the service to stream the response
            await foreach (var sseDto in response)
            {
                // Format according to SSE specification: event: <event>\ndata: <data>\n\n
                var message = $"event: sseDto.Event\ndata: {JsonSerializer.Serialize(sseDto)}\n\n";
                await Response.WriteAsync(message);
                await Response.Body.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"event: SseEvent.Error\ndata: {JsonSerializer.Serialize(new { message = ex.Message })}\n\n";
            await Response.WriteAsync(errorMessage);
            await Response.Body.FlushAsync();
            throw ex;
        }
        finally
        {
            await Response.CompleteAsync();
        }
    }
    [HttpPost]
    public async Task StreamChat([FromBody] ChatRequest request)
    {
        var response = _aiChatAssistant.ChatStreamAsync(request, CancellationToken.None);
        try
        {
            // Use the service to stream the response
            await foreach (var sseDto in response)
            {
                // Format according to SSE specification: event: <event>\ndata: <data>\n\n
                var message = $"event: sseDto.Event\ndata: {JsonSerializer.Serialize(sseDto)}\n\n";
                await Response.WriteAsync(message);
                await Response.Body.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"event: SseEvent.Error\ndata: {JsonSerializer.Serialize(new { message = ex.Message })}\n\n";
            await Response.WriteAsync(errorMessage);
            await Response.Body.FlushAsync();
            throw ex;
        }
        finally
        {
            await Response.CompleteAsync();
        }
    }
    [HttpGet("deepseek-model")]
    public async Task<IActionResult> GetDeepSeekModel()
    {
        var response = await _deepSeekAssistant.GetProviderAsync(CancellationToken.None);
        return Ok(response);
    }
    [HttpGet("ollama-model")]
    public async Task<IActionResult> GetOllamaModel()
    {
        var response = await _ollamaAssistant.GetProviderAsync(CancellationToken.None);
        return Ok(response);
    }
    [HttpGet("providers")]
    public async Task<IActionResult> GetProviders()
    {
        var response = await _aiChatAssistant.GetProviderAsync(CancellationToken.None);
        return Ok(response);
    }
}
