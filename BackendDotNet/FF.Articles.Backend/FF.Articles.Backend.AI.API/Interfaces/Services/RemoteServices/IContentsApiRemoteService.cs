using FF.Articles.Backend.Common.ApiDtos;

namespace FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;

public interface IContentsApiRemoteService
{
    Task<Dictionary<int, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests);
    Task<bool> EditContentBatchAsync(Dictionary<int, string> batchEditConentRequests);
    Task<int> AddTopicByTitleAsync(string title);
    Task<bool> EditArticleByRequest(ArticleApiEditRequest request);
}
