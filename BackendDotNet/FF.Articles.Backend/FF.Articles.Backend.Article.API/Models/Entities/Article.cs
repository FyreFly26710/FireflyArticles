using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Article.API.Models.Entities;
public class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long TopicId { get; set; }
    //public string Tags { get; set; }
    public int SortNumber { get; set; }
}
