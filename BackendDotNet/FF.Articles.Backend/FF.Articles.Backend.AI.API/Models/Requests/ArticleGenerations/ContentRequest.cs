namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class ContentRequest
{
    public long? Id { get; set; }
    public int SortNumber { get; set; }
    public long TopicId { get; set; }
    public string Category { get; set; } = "";
    public string Topic { get; set; } = "";
    public string TopicAbstract { get; set; } = "";
    public string Title { get; set; } = "";
    public string Abstract { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string? Provider { get; set; } = ProviderList.DeepSeek;
}