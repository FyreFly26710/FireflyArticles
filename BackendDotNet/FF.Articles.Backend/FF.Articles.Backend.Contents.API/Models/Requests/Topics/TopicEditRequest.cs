namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics;
public class TopicEditRequest
{
    public long TopicId { get; set; }
    public string? Title { get; set; }
    public string? Abstract { get; set; }
    //public string? Content { get; set; }
    public string? TopicImage { get; set; }
    public string? Category { get; set; }
    public int? SortNumber { get; set; }
    public int? IsHidden { get; set; }
}

