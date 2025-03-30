using System;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.MapperExtensions;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.AI.API.Services.RemoteServices;

public class ContentsApiRemoteService(HttpClient _httpClient) : IContentsApiRemoteService
{
    public async Task<Dictionary<long, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest)
    {
        string url = RemoteApiUrlConstant.ArticleBatchUrl();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, requests, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<long, string>();
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<long, string>>>();
        return res?.Data ?? new();
    }
    public async Task<bool> AddArticleAsync(ArticleApiAddRequest request, HttpRequest httpRequest)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, request, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        return response.IsSuccessStatusCode;
    }
    //public async Task<bool> EditContentBatchAsync(Dictionary<long, string> batchEditConentRequests, HttpRequest httpRequest)
    //{
    //    string url = RemoteApiurlConstant.ArticleBatchEditContenturl();
    //    var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Put, url, batchEditConentRequests, httpRequest);
    //    var response = await _httpClient.SendAsync(httpRequestMessage);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        return false;
    //    }
    //    return true;
    //}
    public async Task<bool> EditArticleByRequest(ArticleApiEditRequest request, HttpRequest httpRequest)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Put, url, request, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        return response.IsSuccessStatusCode;
    }

    public async Task<long> AddTopicByTitleAsync(string title, HttpRequest httpRequest)
    {
        var request = new { title };
        string url = RemoteApiUrlConstant.TopicUrl();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, request, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        return res?.Data ?? 0;
    }

    /// <summary>
    /// Pass AuthCookie to the request
    /// </summary>
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, object content, HttpRequest httpRequest)
    {
        var requestMessage = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(content)
        };

        if (httpRequest.Cookies.TryGetValue("AuthCookie", out var authCookieValue))
        {
            requestMessage.Headers.Add("Cookie", $"AuthCookie={authCookieValue}");
        }

        return requestMessage;
    }
}
