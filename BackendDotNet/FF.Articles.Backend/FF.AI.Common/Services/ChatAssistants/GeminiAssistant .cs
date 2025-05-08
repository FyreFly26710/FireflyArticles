using FF.AI.Common.Models.DeepSeek;
using FF.AI.Common.Models.Gemini;
namespace FF.AI.Common.Services.ChatAssistants;

public class GeminiAssistant : BaseAssistant, IAssistant<GeminiProvider>
{
    public GeminiAssistant(GeminiProvider provider, IHttpClientFactory httpClientFactory) : base(provider, httpClientFactory)
    {
    }
    public async Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var geminiRequest = request.ToGeminiRequest();
        var extraInfo = new ExtraInfo() { CreatedAt = DateTime.UtcNow, Model = request.Model };

        var content = new StringContent(JsonSerializer.Serialize(geminiRequest, _jsonSerializerOptions), Encoding.UTF8, "application/json");
        var url = _provider.ChatEndpoint + "/" + request.Model + ":generateContent?key=" + _provider.ApiKey;
        //var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=AIzaSyDYzrh1_8Y6sxwX6Z1Vki7JVG7DzZFiG-g";
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var resContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new ChatResponse(Message.Assistant(resContent), ChatEvent.Finish, extraInfo);
        }
        if (string.IsNullOrWhiteSpace(resContent))
        {
            return new ChatResponse(Message.Assistant("invalid response"), ChatEvent.Finish, extraInfo);
        }

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(resContent, _jsonSerializerOptions);
        if (geminiResponse is null)
        {
            return new ChatResponse(Message.Assistant("invalid response"), ChatEvent.Finish, extraInfo);
        }

        var chatResponse = geminiResponse.ToChatResponse();
        return chatResponse;
    }
    /// <summary>
    /// Only for article generation, not for chat
    /// </summary>
    public IAsyncEnumerable<ChatResponse>? ChatStreamAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {

        throw new NotImplementedException();
    }
    public async Task<ChatProvider> GetProviderAsync(CancellationToken cancellationToken)
    {
        //https://ai.google.dev/gemini-api/docs/models
        var models = new List<string>()
        {
            "gemini-2.5-flash-preview-04-17",   // 500 RPD
            "gemini-2.0-flash",                 // 1500
            "gemini-2.0-flash-lite"             // 1500

        };
        var chatProvider = new ChatProvider()
        {
            ProviderName = _provider.ProviderName,
            Models = models,
            IsAvailable = false
        };
        return chatProvider;
    }
}