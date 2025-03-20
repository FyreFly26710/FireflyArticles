using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleEditRequest
{
    public int ArticleId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Abstract { get; set; }
    /// <summary>
    ///  Article = 1, SubArticle = 2, TopicArticle = 3
    /// </summary>
    public string? ArticleType { get; set; }
    public int? ParentArticleId { get; set; }
    public int? TopicId { get; set; }
    public List<string>? TagNames { get; set; }
    public int? SortNumber { get; set; }
    public int? IsHidden { get; set; }
}

