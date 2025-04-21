using System;
using FF.AI.Common.Interfaces;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json.Serialization;
namespace FF.AI.Common.Services.ChatAssistants;

public abstract class BaseAssistant
{
    protected readonly IProvider _provider;
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    public BaseAssistant(IProvider provider, IHttpClientFactory httpClientFactory)
    {
        _provider = provider;
        _httpClient = httpClientFactory.CreateClient("ai-client");
        if (_provider.ApiKey is not null)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_provider.ApiKey}");
        }
    }

}
