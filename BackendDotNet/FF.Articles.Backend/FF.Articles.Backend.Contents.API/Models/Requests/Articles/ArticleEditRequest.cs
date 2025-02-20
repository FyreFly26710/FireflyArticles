namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleEditRequest
{
    public int ArticleId { get; set; }
    public string? Title { get; set; } 
    public string? Content { get; set; } 
    public string? Abstraction { get; set; } 
    public int? TopicId { get; set; }
    public List<int>? TagIds { get; set; }
    public int? SortNumber { get; set; }
    public int? IsHidden { get; set; }
}

