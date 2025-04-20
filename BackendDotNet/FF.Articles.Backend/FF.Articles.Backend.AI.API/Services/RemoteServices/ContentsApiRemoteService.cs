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

    public async Task<long> AddArticleAsync(ArticleApiAddRequest payload)
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
        return res?.Data ?? 0;
    }
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, object payload, UserApiDto user)
    {
        var requestMessage = new HttpRequestMessage(method, url) { Content = JsonContent.Create(payload)};
        var adminToken = _tokenService.GenerateApiToken(user);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        return requestMessage;
    }

}
