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
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
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

    public async IAsyncEnumerable<Choice>? ChatStreamAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                    if (!string.IsNullOrWhiteSpace(json) && json != "[DONE]")
                    {
                        var chatResponse = JsonSerializer.Deserialize<ChatResponse>(json, JsonSerializerOptions);
                        var choice = chatResponse?.Choices.FirstOrDefault();
                        if (choice is null)
                        {
                            continue;
                        }
                        yield return choice;
                    }
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

