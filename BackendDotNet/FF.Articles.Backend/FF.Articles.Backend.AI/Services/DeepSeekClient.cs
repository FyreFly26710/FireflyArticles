using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Channels;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;
using Microsoft.Extensions.Configuration;

namespace FF.Articles.Backend.AI.Services;

public class DeepSeekClient : IDeepSeekClient
{
    public DeepSeekClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }
    private readonly string chatEndpoint = ApiEndpoints.DeepSeekChat;
    private readonly string deepSeekBaseAddress = ApiEndpoints.DeepSeekBaseAddress;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public string? ErrorMsg { get; private set; }

    public async Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        request.Stream = false;
        var content = new StringContent(JsonSerializer.Serialize(request, JsonSerializerOptions), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(chatEndpoint, content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var res = await response.Content.ReadAsStringAsync();
            ErrorMsg = response.StatusCode.ToString() + res;
            return null;
        }
        var resContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(resContent))
        {
            ErrorMsg = "empty response";
            return null;
        }
        return JsonSerializer.Deserialize<ChatResponse>(resContent, JsonSerializerOptions);
    }
    /*
    https://api-docs.deepseek.com/api/create-chat-completion

    data: {"id": "1f633d8bfc032625086f14113c411638", "choices": [{"index": 0, "delta": {"content": "", "role": "assistant"}, "finish_reason": null, "logprobs": null}], "created": 1718345013, "model": "deepseek-chat", "system_fingerprint": "fp_a49d71b8a1", "object": "chat.completion.chunk", "usage": null}

    data: {"choices": [{"delta": {"content": "Hello", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": "!", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": " How", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": " can", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": " I", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": " assist", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": " you", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": " today", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": "?", "role": "assistant"}, "finish_reason": null, "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1"}

    data: {"choices": [{"delta": {"content": "", "role": null}, "finish_reason": "stop", "index": 0, "logprobs": null}], "created": 1718345013, "id": "1f633d8bfc032625086f14113c411638", "model": "deepseek-chat", "object": "chat.completion.chunk", "system_fingerprint": "fp_a49d71b8a1", "usage": {"completion_tokens": 9, "prompt_tokens": 17, "total_tokens": 26}}

    data: [DONE]
    */
    public async IAsyncEnumerable<string>? ChatStreamAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        request.Stream = true;
        var content = new StringContent(JsonSerializer.Serialize(request, JsonSerializerOptions), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, chatEndpoint)
        {
            Content = content,
        };
        using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line != null && line.StartsWith("data: "))
                {
                    var json = line.Substring(6);
                    yield return json;
                    // if (!string.IsNullOrWhiteSpace(json) && json != "[DONE]")
                    // {
                    //     var chatResponse = JsonSerializer.Deserialize<ChatResponse>(json, JsonSerializerOptions);
                    //     if (chatResponse is null)
                    //     {
                    //         continue;
                    //     }
                    //     yield return chatResponse;
                    // }
                }
            }
        }
        else
        {
            var res = await response.Content.ReadAsStringAsync();
            ErrorMsg = response.StatusCode.ToString() + res;
            yield break;
        }
    }


}

