using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Contents.API.Models.Entities;
public class Topic:BaseEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Abstraction { get; set; }
    public string Category { get; set; }
    public string? TopicImage { get; set; }
    public int UserId { get; set; }
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}
