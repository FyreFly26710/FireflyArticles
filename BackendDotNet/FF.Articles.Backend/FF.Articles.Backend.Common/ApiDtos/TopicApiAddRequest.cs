namespace FF.Articles.Backend.Common.ApiDtos;

public class TopicApiAddRequest
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
}
