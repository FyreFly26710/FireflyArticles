using System;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using System.Text.Json;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.ApiDtos;
using StackExchange.Redis;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.AI.Common.Interfaces;
using FF.AI.Common.Providers;
using FF.AI.Common.Models;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.AI.Common.Constants;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.RabbitMQ;
using FF.Articles.Backend.AI.API.MapperExtensions;

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
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    // private readonly IDatabase _redis;
    public ArticleGenerationService(IAssistant aiChatAssistant, IContentsApiRemoteService contentsApiRemoteService, ILogger<ArticleGenerationService> logger, IRabbitMqPublisher rabbitMqPublisher)
    {
        _aiChatAssistant = aiChatAssistant;
        _contentsApiRemoteService = contentsApiRemoteService;
        _logger = logger;
        _rabbitMqPublisher = rabbitMqPublisher;
        // _redis = redis.GetDatabase();
    }
    // Round 1
    public async Task<ArticlesAIResponseDto> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default)
    {
        var chatRequest = new ChatRequest
        {
            Model = request.Model!,
            Provider = request.Provider!,
            Messages = {
                Message.User(Prompts.User_ArticleList(request.Topic, request.ArticleCount, request.Category)),
            },
            Options = new ChatOptions() { ResponseFormat = ChatOptions.GetResponseFormat<ArticlesAIResponseDto>() }
        };
        var topicId = await _contentsApiRemoteService.AddTopicByTitleAsync(request.Topic);
        _logger.LogInformation("Begin to generate topic: {topic}; TopicId: {topicId}", request.Topic, topicId);
        // var response = await _aiChatAssistant.ChatAsync(chatRequest, cancellationToken);
        // var jsonContent = response?.Message?.Content ?? "";
        var jsonContent = sampleResponse();
        //_logger.LogInformation("Request to generate topic: {topic}; Milliseconds taken : {time}; Tokens: {tokens}", request.Topic, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);

        // Extract content between first { and last }
        int firstBrace = jsonContent.IndexOf('{');
        int lastBrace = jsonContent.LastIndexOf('}');

        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            jsonContent = jsonContent.Substring(firstBrace, lastBrace - firstBrace + 1);
        }
        //Console.WriteLine(jsonContent);
        ArticlesAIResponseDto? articlesResponse = null;
        try
        {
            articlesResponse = JsonSerializer.Deserialize<ArticlesAIResponseDto>(jsonContent);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, $"Exception: {ex.Message}; AI Response cannot be parsed:{jsonContent}");
        }
        if (articlesResponse == null)
            throw new ApiException(ErrorCode.SYSTEM_ERROR, $"AI Response cannot be parsed:{jsonContent}");
        articlesResponse.TopicId = topicId;
        articlesResponse.Category = request.Category;

        return articlesResponse;
    }

    public async Task<string> GenerateArticleContentAsync(ContentRequest request)
    {
        if (request.TopicId == 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Abstract) || request.Id == null)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");

        var chatRequest = new ChatRequest
        {
            Model = request.Model!,
            Provider = request.Provider!,
            Messages = [
                Message.User(Prompts.User_ArticleContent(request.Category, request.Topic, request.Title, request.Abstract, request.Tags)),
            ],
        };
        _logger.LogInformation("Begin to generate article: {title}", request.Title);
        var response = await _aiChatAssistant.ChatAsync(chatRequest, new CancellationToken());

        var content = response?.Message?.Content ?? "";
        if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

        content = removeOuterFences(content);
        _logger.LogInformation("Request to generate article: {title}; Milliseconds taken : {time}; Tokens: {tokens}", request.Title, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);

        return content;
    }
    /// <summary>
    /// Insert article to db and dispatch article generation to rabbitmq
    /// </summary>
    /// <param name="request"></param>
    /// <returns>article id</returns>
    public async Task<long> DispatchArticleGenerationAsync(ContentRequest request)
    {
        var article = request.ToArticleApiUpsertRequest("Generating content...");
        var articleId = await _contentsApiRemoteService.AddArticleAsync(article);
        request.Id = articleId;
        await _rabbitMqPublisher.PublishAsync(QueueList.GenerateArticleQueue, request);
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

    private string sampleResponse() =>
    """
    {
        "Articles": [
            {
                "SortNumber": 1,
                "Title": "Understanding Prompts in AI: A Beginner's Guide",
                "Abstract": "**What are Prompts?** - Introduction to prompts and their role in AI interactions.  \n**Types of Prompts** - Overview of different prompt styles and their uses.  \n**Creating Effective Prompts** - Basic tips for crafting prompts that yield useful AI responses.",
                "Tags": ["Beginner", "AI Interaction", "N/A", "Overview", "Conversational"]
            },
            {
                "SortNumber": 2,
                "Title": "Advanced Techniques for Prompt Engineering in AI",
                "Abstract": "**Prompt Engineering** - Deep dive into designing prompts for specific AI behaviors.  \n**Optimization Strategies** - Techniques for refining prompts to improve AI output quality.  \n**Case Studies** - Examples of successful prompt engineering in real-world applications.",
                "Tags": ["Advanced", "AI Development", "N/A", "Deep-dive", "Technical"]
            },
            {
                "SortNumber": 3,
                "Title": "The Impact of Prompts on AI Model Performance",
                "Abstract": "**Performance Metrics** - How prompts influence AI model accuracy and efficiency.  \n**Comparative Analysis** - Side-by-side comparison of different prompting strategies.  \n**Future Directions** - Emerging trends in prompt design and their potential impacts.",
                "Tags": ["Expert", "AI Research", "N/A", "Comparison", "Analytical"]
            },
            {
                "SortNumber": 4,
                "Title": "Best Practices for Prompt Design in Conversational AI",
                "Abstract": "**User Experience** - Designing prompts that enhance interaction quality.  \n**Common Pitfalls** - Mistakes to avoid in prompt design.  \n**Recommendations** - Proven strategies for creating effective conversational AI prompts.",
                "Tags": ["General", "Conversational AI", "N/A", "Best-practices", "Conversational"]
            }
        ],
        "AIMessage": "Key subtopics covered include beginner's guide to prompts, advanced prompt engineering techniques, impact of prompts on AI performance, and best practices for prompt design in conversational AI. Completion confirmed."
    }
    """;
}
