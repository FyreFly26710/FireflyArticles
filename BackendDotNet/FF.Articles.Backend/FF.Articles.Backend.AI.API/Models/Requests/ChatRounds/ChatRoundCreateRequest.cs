namespace FF.Articles.Backend.AI.API.Models.Requests.ChatRounds;

public class ChatRoundCreateRequest
{
    // If sessionId is not provided, the session will be created
    public long? SessionId { get; set; }
    public long SessionTimeStamp { get; set; }
    public List<long>? HistoryChatRoundIds { get; set; }
    public string UserMessage { get; set; } = "";
    public string? Model { get; set; } = "deepseek-chat";
    public string? Provider { get; set; } = ProviderList.DeepSeek;
}