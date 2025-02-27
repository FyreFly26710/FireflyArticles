using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.Contents.API.Models.Dtos;
public class ArticleDto
{
    public int ArticleId { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstraction { get; set; }
    /// <summary>
    /// 1: article, 2: sub article, 3: topic article
    /// </summary>
    public string ArticleType { get; set; }
    public int? ParentArticleId { get; set; }
    public List<ArticleDto> SubArticles { get; set; } = new();
    public int UserId { get; set; }
    public UserApiDto? User { get; set; }
    public int TopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}

