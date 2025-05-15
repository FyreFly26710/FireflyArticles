using FF.AI.Common.Models.DeepSeek;
namespace FF.AI.Common.Services.ChatAssistants;

public class DeepSeekAssistant : BaseAssistant, IAssistant<DeepSeekProvider>
{
    public DeepSeekAssistant(DeepSeekProvider provider, IHttpClientFactory httpClientFactory) : base(provider, httpClientFactory)
    {
    }
    public async Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var deepSeekRequest = request.ToDeepSeekRequest();
        deepSeekRequest.Stream = false;
        var extraInfo = new ExtraInfo() { CreatedAt = DateTime.UtcNow, Model = deepSeekRequest.Model };
        var content = new StringContent(JsonSerializer.Serialize(deepSeekRequest, _jsonSerializerOptions), Encoding.UTF8, "application/json");
        try
        {

            var response = await _httpClient.PostAsync(_provider.ChatEndpoint, content, cancellationToken);
            var resContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ChatResponse(Message.Assistant(resContent), ChatEvent.Finish, extraInfo);
            }
            if (string.IsNullOrWhiteSpace(resContent))
            {
                return new ChatResponse(Message.Assistant("invalid response"), ChatEvent.Finish, extraInfo);
            }

            var deepSeekResponse = JsonSerializer.Deserialize<DeepSeekResponse>(resContent, _jsonSerializerOptions);
            if (deepSeekResponse is null)
            {
                return new ChatResponse(Message.Assistant("invalid response"), ChatEvent.Finish, extraInfo);
            }

            extraInfo.InputTokens = deepSeekResponse.Usage?.PromptTokens;
            extraInfo.OutputTokens = deepSeekResponse.Usage?.CompletionTokens;
            extraInfo.Duration = (int)(DateTime.UtcNow - extraInfo.CreatedAt.Value).TotalMilliseconds;
            var chatResponse = new ChatResponse()
            {
                Message = Message.Assistant(deepSeekResponse.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty),
                Event = ChatEvent.Finish,
                ExtraInfo = extraInfo
            };
            return chatResponse;

        }
        catch (Exception ex)
        {
            
            return new ChatResponse(Message.Assistant($"Error Generating Contents: {ex.Message}"), ChatEvent.Finish, extraInfo);
        }
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
    public async IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var deepSeekRequest = request.ToDeepSeekRequest();
        deepSeekRequest.Stream = true;
        deepSeekRequest.StreamOptions = new StreamOptions { IncludeUsage = true };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _provider.ChatEndpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(deepSeekRequest, _jsonSerializerOptions), Encoding.UTF8, "application/json"),
        };

        using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = new ChatResponse
            {
                Message = Message.Assistant($"Error: {response.StatusCode} - {errorContent}"),
                Event = ChatEvent.Error,
                ExtraInfo = new ExtraInfo() { CreatedAt = DateTime.UtcNow, Model = deepSeekRequest.Model }
            };
            yield return errorResponse;
            yield break;
        }

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        var extraInfo = new ExtraInfo() { CreatedAt = DateTime.UtcNow, Model = deepSeekRequest.Model };
        string content = "";
        var startResponse = new ChatResponse
        {
            Message = Message.Assistant(content),
            Event = ChatEvent.Start,
            ExtraInfo = extraInfo
        };
        yield return startResponse;

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            ChatResponse? chatResponse = null;

            try
            {
                var line = await reader.ReadLineAsync();
                if (line == null || !line.StartsWith("data: "))
                    continue;

                var json = line.Substring(6);

                // Handle special case for "[DONE]"
                if (json.Trim() == "[DONE]")
                    continue;

                var deepSeekResponse = JsonSerializer.Deserialize<DeepSeekResponse>(json, _jsonSerializerOptions);
                if (deepSeekResponse is null)
                    continue;

                if (deepSeekResponse.Usage is not null)
                {
                    extraInfo.InputTokens = deepSeekResponse.Usage.PromptTokens;
                    extraInfo.OutputTokens = deepSeekResponse.Usage.CompletionTokens;
                }

                var delta = deepSeekResponse.Choices.FirstOrDefault()?.Delta?.Content ?? string.Empty;
                content += delta;

                chatResponse = new ChatResponse()
                {
                    Message = Message.Assistant(delta),
                    Event = ChatEvent.Generate,
                    ExtraInfo = null
                };
            }
            catch (Exception ex)
            {
                chatResponse = new ChatResponse()
                {
                    Message = Message.Assistant($"Error during streaming: {ex.Message}"),
                    Event = ChatEvent.Error,
                    ExtraInfo = null
                };
            }

            if (chatResponse != null)
            {
                yield return chatResponse;
            }
        }

        extraInfo.Duration = (int)(DateTime.UtcNow - extraInfo.CreatedAt.Value).TotalMilliseconds;
        var endResponse = new ChatResponse
        {
            Message = Message.Assistant(content),
            Event = ChatEvent.Finish,
            ExtraInfo = extraInfo
        };
        yield return endResponse;
    }
    public async Task<ChatProvider> GetProviderAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(_provider.ListModelsEndpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new ChatProvider()
            {
                ProviderName = _provider.ProviderName,
                Models = [],
                IsAvailable = false
            };
        }
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(content);
        var models = document.RootElement.GetProperty("data")
            .EnumerateArray()
            .Select(m => m.GetProperty("id").GetString())
            .Where(id => id != null)
            .ToList();
        var chatProvider = new ChatProvider()
        {
            ProviderName = _provider.ProviderName,
            Models = models,
            IsAvailable = true
        };
        return chatProvider;
    }
}