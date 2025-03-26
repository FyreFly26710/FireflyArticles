using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.Contents.API.Models.Dtos;
public class ArticleDto
{
    public long ArticleId { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstract { get; set; }
    /// <summary>
    /// 1: article, 2: sub article, 3: topic article
    /// </summary>
    public string ArticleType { get; set; }
    public long? ParentArticleId { get; set; }
    public List<ArticleDto> SubArticles { get; set; } = new();
    public long UserId { get; set; }
    public UserApiDto? User { get; set; }
    public long TopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }

}

