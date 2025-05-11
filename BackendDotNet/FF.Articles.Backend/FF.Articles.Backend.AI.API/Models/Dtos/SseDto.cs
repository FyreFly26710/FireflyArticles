namespace FF.Articles.Backend.AI.API.Models.Dtos;

public class SseDto
{
    /// <summary>
    /// Event: generate, data is the content chunk of the article
    /// Event: init, data is ChatRoundDto without content
    /// Event: end, data is full ChatRoundDto
    /// Event: error, data is the error message
    /// </summary>
    public string? Data { get; set; }
    public string? Event { get; set; }
}

public class SseEvent
{
    public const string Init = ChatEvent.Start;
    public const string Data = ChatEvent.Generate;
    public const string End = ChatEvent.Finish;
    public const string Error = ChatEvent.Error;
}
