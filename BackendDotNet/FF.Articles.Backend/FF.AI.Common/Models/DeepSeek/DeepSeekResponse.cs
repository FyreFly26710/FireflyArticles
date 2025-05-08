namespace FF.AI.Common.Models.DeepSeek;

internal class DeepSeekResponse
{
    public string Id { get; set; } = default!;
    public List<Choice> Choices { get; set; } = [];
    public long Created { get; set; }
    public string Model { get; set; } = default!;
    // [JsonPropertyName("system_fingerprint")]
    // public string? SystemFingerprint { get; set; }
    // public string Object { get; set; } = "chat.completion";
    public Usage? Usage { get; set; }
}

public class Choice
{
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
    public int Index { get; set; }
    public Message? Message { get; set; }
    public Message? Delta { get; set; }
    public Logprobs? Logprobs { get; set; }
}

public class Logprobs
{
    public List<ContentLogprob>? Content { get; set; }
}

public class ContentLogprob
{
    public string Token { get; set; } = default!;
    public double Logprob { get; set; }
    public List<int> Bytes { get; set; } = [];
    [JsonPropertyName("top_logprobs")]
    public List<TopLogprob>? TopLogprobs { get; set; }
}

public class TopLogprob
{
    public string Token { get; set; } = default!;
    public double Logprob { get; set; }
    public List<int> Bytes { get; set; } = [];
}

public class Usage
{
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }
    [JsonPropertyName("prompt_cache_hit_tokens")]
    public int PromptCacheHitTokens { get; set; }
    [JsonPropertyName("prompt_cache_miss_tokens")]
    public int PromptCacheMissTokens { get; set; }
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
    [JsonPropertyName("completion_tokens_details")]
    public CompletionTokensDetails? CompletionTokensDetails { get; set; }
}

public class CompletionTokensDetails
{
    [JsonPropertyName("reasoning_tokens")]
    public int ReasoningTokens { get; set; }
}
