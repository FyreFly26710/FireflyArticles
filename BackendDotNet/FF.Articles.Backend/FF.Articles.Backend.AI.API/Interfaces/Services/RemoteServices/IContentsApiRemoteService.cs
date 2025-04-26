using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;

public interface IContentsApiRemoteService
{
    // Task<Dictionary<long, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest);
    Task<long> AddArticleAsync(ArticleApiUpsertRequest payload);
    //Task<bool> EditContentBatchAsync(Dictionary<long, string> batchEditConentRequests, HttpRequest httpRequest);
    Task<long> AddTopicByTitleAsync(string title);
    Task<bool> EditArticleAsync(ArticleApiUpsertRequest payload);
}
