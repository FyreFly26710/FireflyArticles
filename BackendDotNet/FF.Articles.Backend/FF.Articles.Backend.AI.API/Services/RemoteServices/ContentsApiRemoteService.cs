using System;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.MapperExtensions;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;

namespace FF.Articles.Backend.AI.API.Services.RemoteServices;

public class ContentsApiRemoteService(HttpClient _httpClient) : IContentsApiRemoteService
{
    public async Task<Dictionary<int, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests)
    {
        string uri = RemoteApiUriConstant.ArticleBatchUri();
        var response = await _httpClient.PutAsJsonAsync(uri, requests);
        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<int, string>();
        }
        return await response.Content.ReadFromJsonAsync<Dictionary<int, string>>() ?? new Dictionary<int, string>();
    }
    public async Task<bool> EditContentBatchAsync(Dictionary<int, string> batchEditConentRequests)
    {
        string uri = RemoteApiUriConstant.ArticleBatchEditContentUri();
        var response = await _httpClient.PostAsJsonAsync(uri, batchEditConentRequests);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        return true;
    }
    public async Task<bool> EditArticleByRequest(ArticleApiEditRequest request)
    {
        string uri = RemoteApiUriConstant.ArticleUri();
        var response = await _httpClient.PostAsJsonAsync(uri, request);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        return true;
    }

    public async Task<int> AddTopicByTitleAsync(string title)
    {

        var request = new { title };
        string uri = RemoteApiUriConstant.TopicUri();
        var response = await _httpClient.PutAsJsonAsync(uri, request);
        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }
        return await response.Content.ReadFromJsonAsync<int>();
    }
}
