namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics;
public class TopicEditRequest
{
    public int TopicId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Abstraction { get; set; }
    public string? TopicImage { get; set; }
    public int? SortNumber { get; set; }
    public int? IsHidden { get; set; }
}

