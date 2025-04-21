using System;
using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using System.Text.Json;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.ApiDtos;
using StackExchange.Redis;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Providers;
using FF.AI.Common.Models;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.AI.Common.Constants;

namespace FF.Articles.Backend.AI.API.Services;

/// <summary>
/// new topic: - round 1
/// ui inserts topic/category to db.
/// ui sends topic/category and article count. api calls deepseek to generate list of articles (id, title, abstract).
/// returns back to ui along with ai message.
/// 
/// new topic: - round 2
/// ui modifies the list and sends back to api. (either single article or multiple articles)
/// api calls deepseek to generate content, tags of the article. 
/// api calls remote articles api to save new article to db. (save new article, get articleId)
/// return ai message and new address to ui.
/// 
/// existing topic: (update and extend from existing articles) to do.
/// </summary>
public class ArticleGenerationService : IArticleGenerationService
{
    private readonly IAssistant _aiChatAssistant;
    private readonly IContentsApiRemoteService _contentsApiRemoteService;
    private readonly ILogger<ArticleGenerationService> _logger;
    // private readonly IDatabase _redis;
    public ArticleGenerationService(IAssistant aiChatAssistant, IContentsApiRemoteService contentsApiRemoteService, ILogger<ArticleGenerationService> logger)
    {
        _aiChatAssistant = aiChatAssistant;
        _contentsApiRemoteService = contentsApiRemoteService;
        _logger = logger;
        // _redis = redis.GetDatabase();
    }
    // Round 1
    public async Task<ArticlesAIResponseDto> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default)
    {
        // var history = new AiGenHistory
        // {
        //     Topic = request.Topic,
        //     ArticleCount = request.ArticleCount,
        // };

        var chatRequest = new ChatRequest
        {
            Model = request.Model ?? "deepseek-chat",
            Provider = request.Provider ?? ProviderList.DeepSeek,
            Messages = {
                Message.User(Prompts.User_ArticleList(request.Topic, request.ArticleCount)),
            },
            Options = new ChatOptions() { ResponseFormat = ChatOptions.GetResponseFormat<ArticlesAIResponseDto>() }
        };
        var topicId = await _contentsApiRemoteService.AddTopicByTitleAsync(request.Topic);
        _logger.LogInformation("TopicId: {topicId}", topicId);
        var response = await _aiChatAssistant.ChatAsync(chatRequest, cancellationToken);
        var jsonContent = response?.Message?.Content ?? "";
        _logger.LogInformation("Request to generate topic: {topic}; Milliseconds taken : {time}; Tokens: {tokens}", request.Topic, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);

        // Extract content between first { and last }
        int firstBrace = jsonContent.IndexOf('{');
        int lastBrace = jsonContent.LastIndexOf('}');

        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            jsonContent = jsonContent.Substring(firstBrace, lastBrace - firstBrace + 1);
        }
        // Console.WriteLine(jsonContent);
        ArticlesAIResponseDto? articlesResponse = null;
        try
        {
            _logger.LogInformation(jsonContent);
            articlesResponse = JsonSerializer.Deserialize<ArticlesAIResponseDto>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate article:{jsonContent}", jsonContent);
            throw new Exception($"Exception: {ex.Message}; Failed to generate article:{jsonContent}");
        }
        _logger.LogInformation("Convert success");
        articlesResponse.TopicId = topicId;
        // history.AiResponse = jsonContent;
        // history.TopicId = topicId;

        return articlesResponse;
    }

    // round 2
    public async Task<long> GenerateArticleContentAsync(ContentRequest request)
    {
        if (request.TopicId == 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Abstract))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");

        var chatRequest = new ChatRequest
        {
            Model = request.Model ?? "deepseek-chat",
            Provider = request.Provider ?? ProviderList.DeepSeek,
            Messages = [
                Message.User(Prompts.User_ArticleContent(request.Topic, request.Title, request.Abstract, request.Tags)),
            ],
        };

        var response = await _aiChatAssistant.ChatAsync(chatRequest, new CancellationToken());

        var content = response?.Message?.Content ?? "";
        if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

        content = removeOuterFences(content);
        _logger.LogInformation("Request to generate article: {title}; Milliseconds taken : {time}; Tokens: {tokens}", request.Title, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);
        // Add article
        var article = new ArticleApiAddRequest
        {
            Title = request.Title,
            Abstract = request.Abstract,
            Content = content,
            Tags = request.Tags,
            TopicId = request.TopicId,
            SortNumber = request.Id,
            ArticleType = "Article",
            ParentArticleId = null,
        };
        var articleId = await _contentsApiRemoteService.AddArticleAsync(article);

        return articleId;
    }

    private string removeOuterFences(string content)
    {
        bool startsWithMarkdown = content.StartsWith("```markdown");
        bool startsWithFence = !startsWithMarkdown && content.StartsWith("```");
        bool endsWithFence = content.EndsWith("```");
        int startOffset = startsWithMarkdown ? "```markdown".Length :
                          startsWithFence ? "```".Length : 0;
        int endOffset = endsWithFence ? "```".Length : 0;
        if (startOffset > 0 || endOffset > 0)
        {
            content = content.Substring(
                startOffset,
                content.Length - startOffset - endOffset
            ).Trim();
        }
        return content;
    }
}
