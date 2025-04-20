using System;
using System.Text.Json.Serialization;

namespace FF.AI.Common.Models.DeepSeek;

internal class DeepSeekRequest
{
    public List<Message> Messages { get; set; } = [];
    public string Model { get; set; }
    [JsonPropertyName("frequency_penalty")]
    public double? FrequencyPenalty { get; set; }
    [JsonPropertyName("max_tokens")]
    public long? MaxTokens { get; set; }
    [JsonPropertyName("presence_penalty")]
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// type:text or json_object
    /// </summary>
    [JsonPropertyName("response_format")]
    public ResponseFormat? ResponseFormat { get; set; }
    public List<string>? Stop { get; set; }
    public bool? Stream { get; set; }
    [JsonPropertyName("stream_options")]
    public StreamOptions? StreamOptions { get; set; }
    public double? Temperature { get; set; }
    [JsonPropertyName("top_p")]
    public long? TopP { get; set; }
    public List<object>? Tools { get; set; }
    [JsonPropertyName("tool_choice")]
    public string? ToolChoice { get; set; }
    public bool? Logprobs { get; set; }
    [JsonPropertyName("top_logprobs")]
    public int? TopLogprobs { get; set; }
}

public class StreamOptions
{
    // [JsonPropertyName("include_usage")]
    public bool IncludeUsage { get; set; }
}

public class ResponseFormat
{
    public string Type { get; set; } = "text";
}


internal static class DeepSeekRequestExtensions
{
    public static DeepSeekRequest ToDeepSeekRequest(this ChatRequest request)
    {
        var deepSeekRequest = new DeepSeekRequest
        {
            Messages = request.Messages,
            Model = request.Model
        };

        if (request.Options != null)
        {
            if (request.Options.MaxTokens.HasValue)
                deepSeekRequest.MaxTokens = request.Options.MaxTokens.Value;

            if (request.Options.ResponseFormat != null)
                deepSeekRequest.ResponseFormat = new ResponseFormat { Type = "json_object" };

            // if (request.Options.Stream.HasValue)
            //     deepSeekRequest.Stream = request.Options.Stream.Value;

            if (request.Options.Temperature.HasValue)
                deepSeekRequest.Temperature = request.Options.Temperature.Value;
        }

        return deepSeekRequest;
    }
}


