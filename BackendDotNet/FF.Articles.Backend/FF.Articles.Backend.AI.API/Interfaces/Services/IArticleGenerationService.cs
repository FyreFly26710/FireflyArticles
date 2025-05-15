namespace FF.Articles.Backend.AI.API.Interfaces.Services;

public interface IArticleGenerationService
{
    Task<string> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default);
    Task<string> GenerateArticleContentAsync(ContentRequest request);
    Task<string> GenerateTopicContentAsync(TopicApiDto topic);
}
