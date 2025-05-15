using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleAddRequest
{
    public long? ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    /// <summary>
    ///  Article = 1, SubArticle = 2, TopicArticle = 3
    /// </summary>
    public string ArticleType { get; set; } = ArticleTypes.Article;
    public long? ParentArticleId { get; set; }
    public long TopicId { get; set; }
    public long? UserId { get; set; }
    public List<string> Tags { get; set; } = new();
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; } = 0;
}
