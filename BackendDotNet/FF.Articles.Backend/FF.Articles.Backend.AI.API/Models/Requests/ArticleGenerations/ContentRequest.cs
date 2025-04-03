namespace FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

public class ContentRequest
{
    /// <summary>
    /// Used to save new article to db.
    /// </summary>
    public long TopicId { get; set; }
    /// <summary>
    /// To generate old ai message.
    /// </summary>
    public string Topic { get; set; } = "";
    /// <summary>
    /// To generate old ai message.
    /// </summary>
    public int ArticleCount { get; set; }
    /// <summary>
    /// Original ai response message.
    /// </summary>
    public string AiMessage { get; set; } = "";
    /// <summary>
    /// Title of the article from client.
    /// </summary>
    public string Title { get; set; } = "";
    /// <summary>
    /// Abstract of the article from client. (could be modified by client)
    /// </summary>
    public string Abstract { get; set; } = "";
    /// <summary>
    /// Tags of the article from client. (could be modified by client)
    /// </summary>
    public List<string> Tags { get; set; } = new();
    /// <summary>
    /// Sort number of the article.
    /// </summary>
    public int SortNumber { get; set; }
}