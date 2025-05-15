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

    public async Task<ArticleApiDto?> GetArticleById(long articleId)
    {
        var request = new ArticleApiGetRequest
        {
            ArticleId = articleId
        };
        string url = RemoteApiUrlConstant.ArticleUrl(articleId);
        var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, url, request, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleApiDto>>();
        return res?.Data;
    }


    public async Task<long> AddArticleAsync(ArticleApiUpsertRequest payload, UserApiDto user)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, payload, user);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to add article");
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        return res?.Data ?? 0;
    }
    public async Task<bool> EditArticleAsync(ArticleApiUpsertRequest payload, UserApiDto user)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, url, payload, user);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to edit article");
        }
        return true;
    }
    public async Task<bool> EditArticleContentAsync(long articleId, string content)
    {
        string url = RemoteApiUrlConstant.ArticleUrl();
        var payload = new
        {
            ArticleId = articleId,
            Content = content
        };
        var requestMessage = CreateHttpRequestMessage(HttpMethod.Put, url, payload);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to edit article content");
        }
        return true;
    }

    public async Task<long> AddTopic(TopicApiAddRequest payload, UserApiDto user)
    {
        string url = RemoteApiUrlConstant.TopicUrl();

        var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url, payload, user);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to add topic");
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        _logger.LogInformation("AddTopicByTitleAsync: {res}", res);
        return res?.Data ?? 0;
    }
    public async Task<TopicApiDto?> GetTopicById(long topicId, bool isTopicArticle)
    {
        string url = RemoteApiUrlConstant.TopicUrl(topicId);
        var queryParams = new Dictionary<string, bool>
        {
            { "IncludeArticles", isTopicArticle },
            { "IncludeContent", isTopicArticle },
        };
        var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value.ToString().ToLower()}"));
        url = $"{url}?{queryString}";

        var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, url, null, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);
        var response = await _httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var res = await response.Content.ReadFromJsonAsync<ApiResponse<TopicApiDto>>();
        return res?.Data;
    }

    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url, object payload, UserApiDto? user = null)
    {
        var requestMessage = new HttpRequestMessage(method, url);
        if (payload != null)
        {
            requestMessage.Content = JsonContent.Create(payload);
        }
        user ??= AdminUsers.SYSTEM_ADMIN_DEEPSEEK;
        var adminToken = _tokenService.GenerateApiToken(user);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        return requestMessage;
    }
}
