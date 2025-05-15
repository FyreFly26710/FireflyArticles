namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface IArticleGenerationService
{
    Task<string> GenerateArticleListsAsync(ArticleListRequest request);
    Task<string> RegenerateArticleListAsync(ExistingArticleListRequest request, TopicApiDto topic);
    Task<string> GenerateArticleContentAsync(ContentRequest request);
    Task<string> GenerateTopicContentAsync(TopicApiDto topic);
}
