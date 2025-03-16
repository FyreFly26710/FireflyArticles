namespace FF.Articles.Backend.Contents.API.Models.AIIntegration;
public class ArticlesAIResponse
{
    public List<AIGenArticle> Articles = new();
    public required string AIMessage { get; set; }

}
