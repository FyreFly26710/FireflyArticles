namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics;
public class TopicEditRequest
{
    public int TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    public string TopicImage { get; set; } = string.Empty;
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}

