namespace FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;

public interface IContentsApiRemoteService
{
    // Task<Dictionary<long, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest);
    Task<long> AddArticleAsync(ArticleApiUpsertRequest payload, UserApiDto user);
    //Task<bool> EditContentBatchAsync(Dictionary<long, string> batchEditConentRequests, HttpRequest httpRequest);
    Task<long> AddTopic(TopicApiAddRequest payload, UserApiDto user);
    Task<bool> EditArticleAsync(ArticleApiUpsertRequest payload, UserApiDto user);
    Task<ArticleApiDto?> GetArticleById(long articleId);
    Task<TopicApiDto?> GetTopicById(long topicId, bool isTopicArticle);
}
