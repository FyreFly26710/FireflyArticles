namespace FF.Articles.Backend.AI.API.Models.ApiDtos;

public class ArticleApiUpsertRequest
{
    public long? ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string ArticleType { get; set; } = "Article";
    public long? ParentArticleId { get; set; }
    public long TopicId { get; set; }
    public long? UserId { get; set; }
    public List<string>? Tags { get; set; }
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; } = 0;
}

