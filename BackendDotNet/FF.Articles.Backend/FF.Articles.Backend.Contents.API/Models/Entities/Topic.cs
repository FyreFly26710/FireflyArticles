namespace FF.Articles.Backend.Contents.API.Models.Entities;
public class Topic : BaseEntity
{
    public string Title { get; set; }
    public string Abstract { get; set; } = "";
    public string Category { get; set; } = "";
    public string? TopicImage { get; set; }
    public long UserId { get; set; }
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}
