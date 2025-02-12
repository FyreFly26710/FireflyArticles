using FF.Articles.Backend.Common.Dtos;

namespace FF.Articles.Backend.Contents.API.Models.Responses;
public class TopicResponse
{
    public int TopicId { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstraction { get; set; }
    public int UserId { get; set; }
    public UserDto? User { get; set; }
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}

