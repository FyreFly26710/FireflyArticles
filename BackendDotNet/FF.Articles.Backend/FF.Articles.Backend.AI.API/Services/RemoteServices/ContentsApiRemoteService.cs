using System;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.MapperExtensions;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using System.Net.Http.Headers;
namespace FF.Articles.Backend.AI.API.Services.RemoteServices;

public class ContentsApiRemoteService(HttpClient _httpClient, ITokenService _tokenService, ILogger<ContentsApiRemoteService> _logger) : IContentsApiRemoteService
{
    private UserApiDto _systemAdminUser;

    private UserApiDto GetSystemAdminUser()
    {
        // Create the system admin user if it doesn't exist yet
        if (_systemAdminUser == null)
        {
            _systemAdminUser = new UserApiDto
            {
                UserId = -1, // System user ID
                UserName = "deepseek",
                UserRole = UserConstant.ADMIN_ROLE,
                UserAccount = "system_api",
                CreateTime = DateTime.UtcNow
            };
        }

        return _systemAdminUser;
    }

    // public async Task<Dictionary<long, string>> AddBatchArticlesAsync(List<ArticleApiAddRequest> requests, HttpRequest httpRequest)
    // {
    //     string url = RemoteApiUrlConstant.ArticleBatchUrl();
    //     var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, requests, httpRequest);
    //     var response = await _httpClient.SendAsync(httpRequestMessage);

    //     if (!response.IsSuccessStatusCode)
    //     {
    //         return new Dictionary<long, string>();
    //     }
    //     var res = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<long, string>>>();
    //     return res?.Data ?? new();
    // }

    public async Task<long> AddArticleAsync(ArticleApiAddRequest request)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, request, null);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to add article");
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        return res?.Data ?? 0;
    }

    // public async Task<bool> EditArticleByRequest(ArticleApiEditRequest request, HttpRequest httpRequest)
    // {
    //     string url = RemoteApiUrlConstant.ArticleUrl();
    //     var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Put, url, request, httpRequest);
    //     var response = await _httpClient.SendAsync(httpRequestMessage);

    //     return response.IsSuccessStatusCode;
    // }

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
    /// Create a request message with authentication - either using token or passing the cookie
    /// </summary>
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, object content, HttpRequest httpRequest)
    {
        var requestMessage = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(content)
        };

        // Try to get user from request to maintain user context
        if (UserUtil.TryGetUserFromHttpRequest(httpRequest, out var user))
        {
            // Generate a token for this user
            var token = _tokenService.GenerateApiToken(user);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogInformation($"Using authenticated user token for {url}");
        }
        else
        {
            // If we can't get a user (unauthenticated request or error), use system admin
            _logger.LogDebug($"Using system admin token for {url}");
            var adminToken = _tokenService.GenerateApiToken(GetSystemAdminUser());
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        }

        // Fallback to cookie auth if token is somehow not available
        if (requestMessage.Headers.Authorization == null && httpRequest.Cookies.TryGetValue("AuthCookie", out var authCookieValue))
        {
            requestMessage.Headers.Add("Cookie", $"AuthCookie={authCookieValue}");
            _logger.LogWarning($"Falling back to cookie authentication for {url}");
        }

        return requestMessage;
    }
}
