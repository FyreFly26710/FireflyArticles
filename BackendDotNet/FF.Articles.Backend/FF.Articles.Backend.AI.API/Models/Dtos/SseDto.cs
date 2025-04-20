namespace FF.Articles.Backend.AI.API.Models.Dtos;

public class SseDto
{
    public string? Data { get; set; }
    public string? Event { get; set; }
}

public class SseEvent
{
    public const string Init = "init";
    public const string Data = "data";
    public const string End = "end";
    public const string Error = "error";
}
