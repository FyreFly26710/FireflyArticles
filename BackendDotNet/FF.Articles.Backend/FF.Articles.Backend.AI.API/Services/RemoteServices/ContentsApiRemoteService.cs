using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using System.Net.Http.Headers;

namespace FF.Articles.Backend.AI.API.Services.RemoteServices;

public class ContentsApiRemoteService : IContentsApiRemoteService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ContentsApiRemoteService> _logger;

    public ContentsApiRemoteService(
        HttpClient httpClient,
        ITokenService tokenService,
        ILogger<ContentsApiRemoteService> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;

        // Update base address with environment-aware URL
        _httpClient.BaseAddress = new Uri(RemoteApiUrlConstant.GetContentsBaseUrl());
    }

    public async Task<long> AddArticleAsync(ArticleApiUpsertRequest payload)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, payload, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to add article");
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        return res?.Data ?? 0;
    }
    public async Task<bool> EditArticleAsync(ArticleApiUpsertRequest payload)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, url, payload, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to edit article");
        }
        return true;
    }

    public async Task<long> AddTopicByTitleAsync(string title)
    {
        var payload = new { title };
        string url = RemoteApiUrlConstant.TopicUrl();

        var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, payload, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to add topic");
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        _logger.LogInformation("AddTopicByTitleAsync: {res}", res);
        return res?.Data ?? 0;
    }
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, object payload, UserApiDto user)
    {
        var requestMessage = new HttpRequestMessage(method, url);
        if (payload != null)
        {
            requestMessage.Content = JsonContent.Create(payload);
        }
        var adminToken = _tokenService.GenerateApiToken(user);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        return requestMessage;
    }
}
