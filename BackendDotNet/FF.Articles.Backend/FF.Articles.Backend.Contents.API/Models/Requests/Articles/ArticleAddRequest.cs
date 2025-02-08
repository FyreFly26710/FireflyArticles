namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleAddRequest
{
    public string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    public long TopicId { get; set; }
    public List<string> Tags { get; set; }= new ();
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; }= 0;
}
