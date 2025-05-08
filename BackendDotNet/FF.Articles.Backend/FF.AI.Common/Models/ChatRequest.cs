using NJsonSchema;

namespace FF.AI.Common.Models;

public class ChatRequest
{
    public List<Message> Messages { get; set; } = [];
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public ChatOptions? Options { get; set; }

}

public class ChatOptions
{
    public int? MaxTokens { get; set; } = 4096;
    public object? ResponseFormat { get; set; }

    /// <summary>
    /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
    /// </summary>
    public double? Temperature { get; set; }

    public static object GetResponseFormat<T>()
    {
        return JsonSchema.FromType<T>();
    }
}

