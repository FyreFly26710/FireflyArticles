using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Constants;

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
    public int? ParentArticleId { get; set; }
    public int UserId { get; set; }
    public int TopicId { get; set; }
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
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
