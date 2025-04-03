using System;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;
using FF.Articles.Backend.AI.Services;
using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using System.Text.Json;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.AI.API.MapperExtensions;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.AI.API.Services.Stores;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
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
    private readonly IDeepSeekClient _deepSeekClient;
    private readonly IContentsApiRemoteService _contentsApiRemoteService;
    private UserArticleStateStore _userArticleStateStore;

    public ArticleGenerationService(IDeepSeekClient deepSeekClient, IContentsApiRemoteService contentsApiRemoteService, UserArticleStateStore userArticleStateStore)
    {
        _deepSeekClient = deepSeekClient;
        _contentsApiRemoteService = contentsApiRemoteService;
        _userArticleStateStore = userArticleStateStore;
    }
    // Round 1
    public async Task<ArticlesAIResponseDto> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default)
    {

        var chatRequest = new ChatRequest
        {
            Messages = round1_Messages(request.Topic, request.ArticleCount),
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.JsonObject }
        };

        var response = await _deepSeekClient.ChatAsync(chatRequest, cancellationToken);
        var jsonContent = response?.Choices.First().Message?.Content ?? "";
        var articlesResponse = JsonSerializer.Deserialize<ArticlesAIResponseDto>(jsonContent);
        if (articlesResponse is null) throw new Exception("Failed to generate article");

        return articlesResponse;
    }

    // round 2
    public async Task<ArticleApiAddRequest> GenerateArticleContentAsync(ContentRequest request, HttpRequest httpRequest)
    {
        var round1 = round1_Messages(request.Topic, request.ArticleCount);

        var chatRequest = new ChatRequest
        {
            Messages = [
                ..round1,
                Message.NewAssistantMessage(request.AiMessage),
                Message.NewSystemMessage(format_ArticleContent),
                Message.NewUserMessage("You are a helpful assistant that generates articles based on the following information: "+request.Abstract),
            ],
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.Text }
        };

        var response = await _deepSeekClient.ChatAsync(chatRequest, new CancellationToken());

        var content = response?.Choices.First().Message?.Content ?? "";
        if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

        // Add article
        var article = new ArticleApiAddRequest
        {
            Title = request.Title,
            Abstract = request.Abstract,
            Content = content,
            Tags = request.Tags,
            TopicId = request.TopicId,
            SortNumber = request.SortNumber,
            ArticleType = "Article",
            ParentArticleId = null,
        };
        await _contentsApiRemoteService.AddArticleAsync(article, httpRequest);

        return article;
    }

    // public async Task BatchGenerateArticleContentAsync(List<int> articleIds, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    // {
    //     var tasks = articleIds
    //         .Select(id => GenerateArticleContentAsync(id, httpRequest, cancellationToken))
    //         .ToList();

    //     await Task.WhenAll(tasks);
    // }

    private List<Message> round1_Messages(string topic, int articleCount) => [
        Message.NewSystemMessage(format_ArticlesList),
        Message.NewSystemMessage(system_ArticleList),
        Message.NewUserMessage(@$"Topic: {topic}; ArticleCount: {articleCount}"),
    ];
    private string system_ArticleList = """
    Take a deep breath.
    Think step by step.
    You are a helpful assistant that generates articles based on the given title and content
    First, user will give you {topic} and {articleCount}. 
    For the first message, You will respond using the following rule:
        Generate {articleCount} article titles covering the {topic} from beginner to expert level.  

        Title: Meaningful title for articles
        Abstract:
            - Provide 2-3 key points, each in a single concise sentence. 
            - It does not need to be proper full sentences. Just key words and very brief explanation.
        Tags:
            - Provide 2-4 tags for the article.
        Each title should explore a distinct subtopic or angle related to the main topic.
        Do not write too much articles about beginner levels.
     
        **Example Output**:
        Title: Rapid HTML Enhancement & Best Practices
        Abstract:
        Semantic HTML - Using elements that convey meaning for better accessibility and SEO.
        Metadata & SEO - `<meta> tags`, `<title>`, how HTML influences search engines.
        Accessibility Considerations - `aria-*` attributes, proper use of headings and labels.
        Tags: html, seo, web development
    """;

    private string format_ArticlesList => """
    Generate a JSON response in this EXACT structure:
    {
    "Articles": [
        {
        "Id": 1,
        "Title": "Article Title",
        "Abstract": "**Key Point** - content with two trailing spaces.  **Key Point** - content with two trailing spaces.  **Key Point** - content with two trailing spaces.  ",
        "Tags": ["tag1", "tag2", "tag3"]
        },
        { article2 etc... }
    ],
    "AIMessage": "Your message here"
    }

    **RULES:**
    - Property names MUST be "Articles", "Id", "Title", "Abstract", "Tags", "AIMessage" (case-sensitive)
    1. **Id:** Start at 1 and increment for each article.
    2. **Title:** Clear and concise, describing the article's focus.
    3. **Abstract:** **KeyPoints**: one sentence briefly summarizing each points.
    4. **Tags:** List of tags for the article, 2-4 tags.
    5. **AIMessage:** 
        - If the topic is valid: List key subtopics covered and confirm completion.
        - If invalid/off-topic: Leave `Articles` empty and explain why in `AIMessage`.
    - **JSON Syntax:** Ensure all quotes, commas, and brackets are closed.
    """;
    private string format_ArticleContent => """
    Respond ONLY with the raw markdown content following this exact structure:
    
        # Introduction
        Content...

        ## [First Key Point]
        Content...

        ## [Second Key Point] 
        Content...

        # Conclusion
        Content...
    
    Introduction and Conclusion contents should be 1 paragraphs.
    Key Points should be 1-3 paragraphs.

    DO NOT include: 
    - Title
    - Markdown code fences
    - Any text outside the specified structure
    """;
}
