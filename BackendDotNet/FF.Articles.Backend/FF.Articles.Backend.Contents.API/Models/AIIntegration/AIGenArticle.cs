using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Models.AIIntegration;

public class AIGenArticle
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public required string Abstract { get; set; }
    public int TopicId { get; set; }
    public int SortNumber { get; set; }
}

