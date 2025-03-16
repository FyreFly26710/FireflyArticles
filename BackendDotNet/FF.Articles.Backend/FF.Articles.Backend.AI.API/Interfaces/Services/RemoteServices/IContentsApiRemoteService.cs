    using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;

public interface IContentsApiRemoteService
{
    Task<Dictionary<int, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest);
    Task<bool> EditContentBatchAsync(Dictionary<int, string> batchEditConentRequests, HttpRequest httpRequest);
    Task<int> AddTopicByTitleAsync(string title, HttpRequest httpRequest);
    Task<bool> EditArticleByRequest(ArticleApiEditRequest request, HttpRequest httpRequest);
}
