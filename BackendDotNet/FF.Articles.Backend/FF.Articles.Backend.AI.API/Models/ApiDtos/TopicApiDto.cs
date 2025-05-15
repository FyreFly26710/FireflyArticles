namespace FF.Articles.Backend.AI.API.Models.ApiDtos;

public class TopicApiDto
{
    public long TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    // public string TopicImage { get; set; } = string.Empty;
    // public long UserId { get; set; }
    // public UserApiDto? User { get; set; }
    // public int SortNumber { get; set; }
    // public int IsHidden { get; set; }
    public List<ArticleApiDto>? Articles { get; set; }
}
