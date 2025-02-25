using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Models.Entities;
public class Article : BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstraction { get; set; }
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
