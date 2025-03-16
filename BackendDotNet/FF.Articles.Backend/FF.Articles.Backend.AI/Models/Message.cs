using System;
using System.Text.Json.Serialization;
using FF.Articles.Backend.AI.Constants;

namespace FF.Articles.Backend.AI.Models;

public class Message
{

    public string Content { get; set; } = string.Empty;
    public string Role { get; set; } = MessageRoles.User;
    public string? Name { get; set; }

    /// <summary>
    /// beta feature
    /// </summary>
    // public bool? Prefix { get; set; }

    /// <summary>
    /// beta feature
    /// </summary>
    // [JsonPropertyName("reasoning_content")]
    // public string? ReasoningContent { get; set; }


    public static Message NewUserMessage(string content)
    {
        return new Message
        {
            Content = content,
            Role = MessageRoles.User
        };
    }

    public static Message NewSystemMessage(string content)
    {
        return new Message
        {
            Content = content,
            Role = MessageRoles.System
        };
    }

    public static Message NewAssistantMessage(string content, bool? prefix = null, string? reasoningContent = null)
    {
        return new Message
        {
            Content = content,
            Role = MessageRoles.Assistant,
            // Prefix = prefix,
            // ReasoningContent = reasoningContent
        };
    }

}
