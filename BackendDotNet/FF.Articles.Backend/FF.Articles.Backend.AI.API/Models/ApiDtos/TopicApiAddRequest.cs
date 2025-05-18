namespace FF.Articles.Backend.AI.API.Models.ApiDtos;

public class TopicApiAddRequest
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string? TopicImage { get; set; }
}
