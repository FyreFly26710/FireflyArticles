namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class ExistingArticleListRequest
{
    public long TopicId { get; set; }
    public int ArticleCount { get; set; } = 5;
    public string? UserPrompt { get; set; }
}

