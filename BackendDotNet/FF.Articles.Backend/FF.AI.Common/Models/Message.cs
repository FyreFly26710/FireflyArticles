using System;
using FF.AI.Common.Constants;

namespace FF.AI.Common.Models;
public class Message
{
    public string Content { get; set; } = string.Empty;
    public string Role { get; set; } = MessageRoles.User;

    public static Message User(string content)
    {
        return new Message { Content = content, Role = MessageRoles.User };
    }
    public static Message System(string content)
    {
        return new Message { Content = content, Role = MessageRoles.System };
    }
    public static Message Assistant(string content)
    {
        return new Message { Content = content, Role = MessageRoles.Assistant };
    }

}
