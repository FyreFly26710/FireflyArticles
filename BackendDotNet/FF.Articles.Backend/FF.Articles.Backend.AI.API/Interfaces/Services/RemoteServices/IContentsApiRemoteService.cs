using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;

public interface IContentsApiRemoteService
{
    Task<Dictionary<long, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest);
    Task<bool> AddArticleAsync(ArticleApiAddRequest request, HttpRequest httpRequest);
    //Task<bool> EditContentBatchAsync(Dictionary<long, string> batchEditConentRequests, HttpRequest httpRequest);
    Task<long> AddTopicByTitleAsync(string title, HttpRequest httpRequest);
    Task<bool> EditArticleByRequest(ArticleApiEditRequest request, HttpRequest httpRequest);
}
