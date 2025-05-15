namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class TopicArticleContentRequest
{
    public long ArticleId { get; set; }
    public string? UserPrompt { get; set; }
}

