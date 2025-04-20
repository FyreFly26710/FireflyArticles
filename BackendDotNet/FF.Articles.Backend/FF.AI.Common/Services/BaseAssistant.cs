using System;
using FF.AI.Common.Interfaces;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using FF.AI.Common.Models;
namespace FF.AI.Common.Services;

public abstract class BaseAssistant
{
    protected readonly IProvider _provider;
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
    public BaseAssistant(IProvider provider, HttpClient httpClient)
    {
        _provider = provider;
        _httpClient = httpClient;
        if (_provider.ApiKey is not null)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_provider.ApiKey}");
        }
    }

}
