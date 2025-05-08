namespace FF.AI.Common.Models.Gemini;
internal class Content
{
    public string? Role { get; set; }
    public Part[] Parts { get; set; } = [];
}
internal class Part
{
    public string Text { get; set; } = string.Empty;
}

internal static class ContentExtensions
{
    public static Message ToMessage(this Content content)
    {
        var message = new Message();
        message.Role = content.Role == "user" ? MessageRoles.User : MessageRoles.Assistant;
        message.Content = content.Parts.First().Text;
        return message;
    }
    public static Content ToContent(this Message message)
    {
        var content = new Content
        {
            Role = message.Role == MessageRoles.User ? "user" : "model",
            Parts = new[] { new Part { Text = message.Content } }
        };
        return content;
    }
}
