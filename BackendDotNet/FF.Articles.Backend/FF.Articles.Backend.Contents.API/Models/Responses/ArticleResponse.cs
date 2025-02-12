using FF.Articles.Backend.Common.Dtos;

namespace FF.Articles.Backend.Contents.API.Models.Responses;
public class ArticleResponse
{
    public int ArticleId { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstraction { get; set; }
    public int UserId { get; set; }
    public UserDto? User { get; set; }
    public int TopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new ();
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}

