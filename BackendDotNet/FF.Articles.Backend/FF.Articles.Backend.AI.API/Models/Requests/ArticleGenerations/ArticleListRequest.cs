namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class ArticleListRequest
{
    public string Topic { get; set; } = "";
    public int ArticleCount { get; set; } = 8;
}