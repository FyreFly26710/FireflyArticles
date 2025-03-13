using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.Contents.API.Models.Dtos;
public class TopicDto
{
    public int TopicId { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string TopicImage { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserApiDto? User { get; set; }
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
    public List<ArticleDto>? Articles { get; set; }
}

