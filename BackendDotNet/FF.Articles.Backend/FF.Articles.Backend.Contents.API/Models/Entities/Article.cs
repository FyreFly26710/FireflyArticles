namespace FF.Articles.Backend.Contents.API.Models.Entities;
public class Article : BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; } = "";
    public string Abstract { get; set; } = "";
    /// <summary>
    ///  Article = 1, SubArticle = 2, TopicArticle = 3
    /// </summary>
    public string ArticleType { get; set; } = ArticleTypes.Article;
    public long? ParentArticleId { get; set; }
    public long UserId { get; set; }
    public long TopicId { get; set; }
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }

    public Article Clone()
    {
        return new Article
        {
            Title = Title,
            Content = Content,
            Abstract = Abstract,
            ArticleType = ArticleType,
            ParentArticleId = ParentArticleId,
            UserId = UserId,
            TopicId = TopicId,
            SortNumber = SortNumber,
            IsHidden = IsHidden,
            CreateTime = CreateTime,
            UpdateTime = UpdateTime,
            IsDelete = IsDelete
        };
    }
}

public static class ArticleProperty
{
    public const string Title = "Title";
    public const string Content = "Content";
    public const string Abstract = "Abstract";
    public const string ArticleType = "ArticleType";
    public const string ParentArticleId = "ParentArticleId";
    public const string UserId = "UserId";
    public const string TopicId = "TopicId";
    public const string SortNumber = "SortNumber";
    public const string IsHidden = "IsHidden";
    public const string CreateTime = "CreateTime";
    public const string UpdateTime = "UpdateTime";
    public const string IsDelete = "IsDelete";
}
