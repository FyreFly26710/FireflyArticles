namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleEditRequest
{
    public int ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public List<int> TagIds { get; set; } = new();
    public int SortNumber { get; set; }
    public int IsHidden { get; set; }
}

