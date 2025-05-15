namespace FF.Articles.Backend.AI.API.Models.ApiDtos;
public class ArticleApiDto
{
    public long ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstract { get; set; }
    public string ArticleType { get; set; }
    public long? ParentArticleId { get; set; }
    public List<ArticleApiDto> SubArticles { get; set; } = new();
    public long UserId { get; set; }
    public UserApiDto? User { get; set; }
    public long TopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

}
public static class ArticleTypes
{
    public const string Article = "Article";
    public const string SubArticle = "SubArticle";
    public const string TopicArticle = "TopicArticle";
}

