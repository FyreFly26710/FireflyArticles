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
}