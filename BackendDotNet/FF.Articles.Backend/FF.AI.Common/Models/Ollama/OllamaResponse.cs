using System;
using System.Text.Json.Serialization;
namespace FF.AI.Common.Models.Ollama;

internal class OllamaResponse
{
    /// <summary>
    /// The name of the model used for generation.
    /// Example: "deepseek-r1:1.5b"
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; set; }

    /// <summary>
    /// The timestamp when the response was created, in ISO 8601 format.
    /// Example: "2025-04-20T10:19:05.029122Z"
    /// </summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = default!;

    /// <summary>
    /// The message containing the response content and role
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    /// <summary>
    /// The reason why generation completed.
    /// Example: "stop"
    /// </summary>
    [JsonPropertyName("done_reason")]
    public string? DoneReason { get; set; }

    /// <summary>
    /// Indicates whether this is the final response.
    /// </summary>
    [JsonPropertyName("done")]
    public bool Done { get; set; }

    /// <summary>
    /// Total duration of the entire generation process in nanoseconds.
    /// </summary>
    [JsonPropertyName("total_duration")]
    public long TotalDuration { get; set; }

    /// <summary>
    /// Time spent loading the model into memory in nanoseconds.
    /// </summary>
    [JsonPropertyName("load_duration")]
    public long LoadDuration { get; set; }

    /// <summary>
    /// Number of tokens in the prompt/input messages.
    /// </summary>
    [JsonPropertyName("prompt_eval_count")]
    public int PromptEvalCount { get; set; }

    /// <summary>
    /// Time spent processing the prompt/input in nanoseconds.
    /// </summary>
    [JsonPropertyName("prompt_eval_duration")]
    public long PromptEvalDuration { get; set; }

    /// <summary>
    /// Number of tokens in the generated response.
    /// </summary>
    [JsonPropertyName("eval_count")]
    public int EvalCount { get; set; }

    /// <summary>
    /// Time spent generating the response in nanoseconds.
    /// </summary>
    [JsonPropertyName("eval_duration")]
    public long EvalDuration { get; set; }
}
