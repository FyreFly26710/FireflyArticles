using FF.AI.Common.Constants;

namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class ArticleListRequest
{
    public string Category { get; set; } = "";
    public string Topic { get; set; } = "";
    public string TopicAbstract { get; set; } = "";
    public int ArticleCount { get; set; } = 8;
    public string? Provider { get; set; } = ProviderList.DeepSeek;
}