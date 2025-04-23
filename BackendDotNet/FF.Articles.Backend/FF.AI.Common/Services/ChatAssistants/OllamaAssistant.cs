using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using FF.AI.Common.Constants;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Models;
using FF.AI.Common.Models.Ollama;
using System.Text.Json.Serialization;
using System.Text;
using System.Runtime.CompilerServices;
using FF.AI.Common.Providers;

namespace FF.AI.Common.Services.ChatAssistants;

public class OllamaAssistant : BaseAssistant, IAssistant<OllamaProvider>
{
    public OllamaAssistant(OllamaProvider provider, IHttpClientFactory httpClientFactory) : base(provider, httpClientFactory)
    {
    }

    public async Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {

        var ollamaRequest = request.ToOllamaRequest();
        ollamaRequest.Stream = false;
        var extraInfo = new ExtraInfo() { CreatedAt = DateTime.UtcNow, Model = ollamaRequest.Model };
        var content = new StringContent(JsonSerializer.Serialize(ollamaRequest, _jsonSerializerOptions), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_provider.ChatEndpoint, content, cancellationToken);
        var resContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new ChatResponse(Message.Assistant(resContent), ChatEvent.Finish, extraInfo);
        }

        var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(resContent, _jsonSerializerOptions);
        if (ollamaResponse is null)
        {
            return new ChatResponse(Message.Assistant(resContent), ChatEvent.Finish, extraInfo);
        }

        extraInfo.InputTokens = ollamaResponse.PromptEvalCount;
        extraInfo.OutputTokens = ollamaResponse.EvalCount;
        extraInfo.Duration = (int)(DateTime.UtcNow - extraInfo.CreatedAt.Value).TotalMilliseconds;
        var message = Message.Assistant(ollamaResponse?.Message?.Content ?? string.Empty);
        var chatResponse = new ChatResponse(message, ChatEvent.Finish, extraInfo);
        return chatResponse;

    }

    /*
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:13.942379Z","message":{"role":"assistant","content":"\u003cthink\u003e"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:13.965273Z","message":{"role":"assistant","content":"\n\n"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:13.988737Z","message":{"role":"assistant","content":"\u003c/think\u003e"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.01192Z","message":{"role":"assistant","content":"\n\n"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.036129Z","message":{"role":"assistant","content":"Hello"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.061459Z","message":{"role":"assistant","content":"!"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.086571Z","message":{"role":"assistant","content":" How"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.112609Z","message":{"role":"assistant","content":" can"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.138158Z","message":{"role":"assistant","content":" I"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.163653Z","message":{"role":"assistant","content":" assist"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.188551Z","message":{"role":"assistant","content":" you"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.214118Z","message":{"role":"assistant","content":" today"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.239536Z","message":{"role":"assistant","content":"?"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.290952Z","message":{"role":"assistant","content":" 😊"},"done":false}
{"model":"deepseek-r1:1.5b","created_at":"2025-04-20T10:23:14.317428Z","message":{"role":"assistant","content":""},"done_reason":"stop","done":true,"total_duration":476296875,"load_duration":29046166,"prompt_eval_count":4,"prompt_eval_duration":70444125,"eval_count":16,"eval_duration":376344542}

    */
    public async IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var ollamaRequest = request.ToOllamaRequest();
        ollamaRequest.Stream = true;
        var extraInfo = new ExtraInfo() { CreatedAt = DateTime.UtcNow, Model = ollamaRequest.Model };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _provider.ChatEndpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(ollamaRequest, _jsonSerializerOptions), Encoding.UTF8, "application/json"),
        };
        using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var res = await response.Content.ReadAsStringAsync();
            yield return new ChatResponse(Message.Assistant(res), ChatEvent.Finish, extraInfo);
        }
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var content = "";
        yield return new ChatResponse(Message.Assistant(content), ChatEvent.Start, extraInfo);

        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line != null)
            {
                var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(line, _jsonSerializerOptions);
                if (ollamaResponse is null) continue;

                if (ollamaResponse.PromptEvalCount > 0)
                {
                    extraInfo.InputTokens = ollamaResponse.PromptEvalCount;
                }
                if (ollamaResponse.EvalCount > 0)
                {
                    extraInfo.OutputTokens = ollamaResponse.EvalCount;
                }
                var chunk = ollamaResponse.Message?.Content ?? string.Empty;
                content += chunk;
                yield return new ChatResponse(Message.Assistant(chunk), ChatEvent.Generate, extraInfo);
            }
        }
        extraInfo.Duration = (int)(DateTime.UtcNow - extraInfo.CreatedAt.Value).TotalMilliseconds;
        yield return new ChatResponse(Message.Assistant(content), ChatEvent.Finish, extraInfo);
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
        var models = document.RootElement.GetProperty("models")
            .EnumerateArray()
            .Select(m => m.GetProperty("name").GetString())
            .Where(name => name != null)
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