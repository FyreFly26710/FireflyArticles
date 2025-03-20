using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics;
public class TopicAddRequest
{
    public string Title { get; set; }
    public string Abstract { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TopicImage { get; set; } = string.Empty;
    public string Category { get; set; } = TopicCategories.Other;
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; } = 0;
}
