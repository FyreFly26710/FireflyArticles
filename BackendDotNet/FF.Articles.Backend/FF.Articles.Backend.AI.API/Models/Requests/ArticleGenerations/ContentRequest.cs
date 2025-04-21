using FF.AI.Common.Constants;

namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class ContentRequest
{
    public int Id { get; set; }
    public long TopicId { get; set; }
    public string Topic { get; set; } = "";
    public string Title { get; set; } = "";
    public string Abstract { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string? Model { get; set; } = "deepseek-chat";
    public string? Provider { get; set; } = ProviderList.DeepSeek;
}