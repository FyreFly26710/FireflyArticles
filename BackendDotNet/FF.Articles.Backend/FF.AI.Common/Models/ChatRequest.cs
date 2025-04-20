using System;

namespace FF.AI.Common.Models;

public class ChatRequest
{
    public List<Message> Messages { get; set; } = [];
    public string Model { get; set; } = "deepseek-chat";
    public ChatOptions? Options { get; set; }

}

public class ChatOptions
{
    public int? MaxTokens { get; set; } = 4096;

    /// <summary>
    /// type:text or json_object
    /// </summary>
    public string? ResponseFormat { get; set; }
    // public bool? Stream { get; set; }
    /// <summary>
    /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
    /// </summary>
    public double? Temperature { get; set; }
}

