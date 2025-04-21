using System;
using System.Text.Json.Serialization;
namespace FF.AI.Common.Models.Ollama;

internal class OllamaRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    [JsonPropertyName("messages")]
    public required List<Message> Messages { get; set; }

    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }

    /// <summary>
    /// Optional format for response (e.g., "json" or a JSON schema object).
    /// </summary>
    [JsonPropertyName("format")]
    public object? Format { get; set; }

    /// <summary>
    /// Optional advanced parameters for the model.
    /// </summary>
    [JsonPropertyName("options")]
    public OllamaChatOptions? Options { get; set; }

}

/// <summary>
/// Options for customizing the behavior of Ollama models.
/// </summary>
public class OllamaChatOptions
{
    /// <summary>
    /// Controls randomness. Higher values (e.g., 1.0) make output more random, 
    /// while lower values (e.g., 0.1) make it more deterministic.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    /// <summary>
    /// Maximum number of tokens to predict.
    /// </summary>
    [JsonPropertyName("num_predict")]
    public int? NumPredict { get; set; }

    /// <summary>
    /// Only sample from the top K options for each subsequent token.
    /// </summary>
    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    /// <summary>
    /// Select from tokens with cumulative probability above a threshold P.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    /// <summary>
    /// Penalty applied to repeated tokens.
    /// </summary>
    [JsonPropertyName("repeat_penalty")]
    public double? RepeatPenalty { get; set; }

    /// <summary>
    /// Sets the random number seed to use for generation.
    /// </summary>
    [JsonPropertyName("seed")]
    public int? Seed { get; set; }
}

internal static class OllamaRequestExtensions
{
    public static OllamaRequest ToOllamaRequest(this ChatRequest request)
    {
        var ollamaRequest = new OllamaRequest
        {
            Messages = request.Messages,
            Model = request.Model
        };

        if (request.Options != null)
        {
            ollamaRequest.Options = new OllamaChatOptions();
            if (request.Options.MaxTokens.HasValue)
                ollamaRequest.Options.NumPredict = request.Options.MaxTokens.Value;

            if (request.Options.ResponseFormat != null)
                ollamaRequest.Format = request.Options.ResponseFormat;

            // if (request.Options.Stream.HasValue)
            //     deepSeekRequest.Stream = request.Options.Stream.Value;

            if (request.Options.Temperature.HasValue)
                ollamaRequest.Options.Temperature = request.Options.Temperature.Value;
        }

        return ollamaRequest;
    }
}


