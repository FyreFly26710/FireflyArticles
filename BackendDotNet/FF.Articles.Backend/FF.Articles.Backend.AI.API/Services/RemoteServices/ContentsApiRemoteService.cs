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
    public async Task<Dictionary<int, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest)
    {
        string uri = RemoteApiUriConstant.ArticleBatchUri();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, requests, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return new Dictionary<int, string>();
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<int, string>>>();
        return  res?.Data ?? new();
    }
    public async Task<bool> EditContentBatchAsync(Dictionary<int, string> batchEditConentRequests, HttpRequest httpRequest)
    {
        string uri = RemoteApiUriConstant.ArticleBatchEditContentUri();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, batchEditConentRequests, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        return true;
    }
    public async Task<bool> EditArticleByRequest(ArticleApiEditRequest request, HttpRequest httpRequest)
    {
        string uri = RemoteApiUriConstant.ArticleUri();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, uri, request, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }
        return true;
    }

    public async Task<int> AddTopicByTitleAsync(string title, HttpRequest httpRequest)
    {
        var request = new { title };
        string uri = RemoteApiUriConstant.TopicUri();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Put, uri, request, httpRequest);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();
        return res?.Data ?? 0;
    }

    /// <summary>
    /// Pass AuthCookie to the request
    /// </summary>
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string uri, object content, HttpRequest httpRequest)
    {
        var requestMessage = new HttpRequestMessage(method, uri)
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
