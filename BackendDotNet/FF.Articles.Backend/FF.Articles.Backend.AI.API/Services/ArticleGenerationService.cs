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
namespace FF.Articles.Backend.AI.API.Services;

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
    public async Task<ArticlesAIResponse> GenerateArticleListsAsync(string topic, HttpRequest httpRequest, int articleCount = 8, CancellationToken cancellationToken = default)
    {
        var user = UserUtil.GetUserFromHttpRequest(httpRequest);
        var userArticleState = _userArticleStateStore.GetOrAddState(user.UserId);

        userArticleState.HistroyMessages = [
                Message.NewSystemMessage(format_ArticlesList),
                Message.NewSystemMessage(system_ArticleList),
                Message.NewUserMessage(@$"Topic: {topic}; ArticleCount: {articleCount}"),
            ];

        var request = new ChatRequest
        {
            Messages = userArticleState.HistroyMessages,
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.JsonObject }
        };

        var response = await _deepSeekClient.ChatAsync(request, cancellationToken);
        var jsonContent = response?.Choices.First().Message?.Content ?? "";
        var articlesResponse = JsonSerializer.Deserialize<ArticlesAIResponse>(jsonContent);
        if (articlesResponse is null) throw new Exception("Failed to generate article");

        // Insert topic
        var topicId = await _contentsApiRemoteService.AddTopicByTitleAsync(topic, httpRequest);

        // Update state
        userArticleState.ArticlesAIResponse = articlesResponse;
        userArticleState.Topic = topic;
        userArticleState.IsFirstRound = false;
        userArticleState.ApiAddRequests = articlesResponse.Articles.Select(a=>a.ToArticleApiRequest(topicId)).ToList();

        return articlesResponse;
    }

    // round 2
    public async Task<ArticleApiAddRequest> GenerateArticleContentAsync(int articleId, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        var user = UserUtil.GetUserFromHttpRequest(httpRequest);
        if (user is null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Please login");

        var userArticleState = _userArticleStateStore.GetOrAddState(user.UserId);
        if (userArticleState.IsFirstRound) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Please generate article lists first");

        var selectedArticle = userArticleState.ApiAddRequests.FirstOrDefault(a => a.SortNumber == articleId);
        if (selectedArticle is null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to find article");

        var request = new ChatRequest
        {
            Messages = [
                //..userArticleState.HistroyMessages,
                Message.NewUserMessage(user_ArticleContent(selectedArticle.Title, selectedArticle.Abstract)),
                Message.NewSystemMessage(format_ArticleContent),
            ],
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.Text }
        };

        var response = await _deepSeekClient.ChatAsync(request, cancellationToken);

        var content = response?.Choices.First().Message?.Content ?? "";
        if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

        // Add article
        selectedArticle.Content = content;
        await _contentsApiRemoteService.AddArticleAsync(selectedArticle, httpRequest);

        return selectedArticle;
    }

    public async Task BatchGenerateArticleContentAsync(List<int> articleIds, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        var tasks = articleIds
            .Select(id => GenerateArticleContentAsync(id, httpRequest, cancellationToken))
            .ToList();

        await Task.WhenAll(tasks);
    }


    private string system_ArticleList = """
    You are a helpful assistant that generates articles based on the given title and content
    First, user will give you {topic} and {articleCount}. 
    For the first message, You will respond using the following rule:
        Generate {articleCount} article titles covering the {topic} from beginner to expert level.  

        Title: Meaningful title for articles
        Abstract:
            - Provide 2-3 key points, each in a single concise sentence. 
            - It does not need to be proper full sentences. Just key words and very brief explanation.
        Tags: 2-4 relevant keywords or phrases.

        Each title should explore a distinct subtopic or angle related to the main topic.
        Do not write too much articles about beginner levels.
     
        **Example Output Format**:
        Title: Rapid HTML Enhancement & Best Practices
        Abstract:
        Semantic HTML - Using elements that convey meaning for better accessibility and SEO.
        Metadata & SEO - `<meta> tags`, `<title>`, how HTML influences search engines.
        Accessibility Considerations - `aria-*` attributes, proper use of headings and labels.
    """;


    private string user_ArticleContent(string title, string keyPoints) => @$"
    You are a helpful assistant that generates articles based on the title: {title} and key points: {keyPoints}  
    Content Generation Guidelines:
    - Write in professional but accessible tone
    - Maintain academic rigor without jargon
    - Ensure logical flow between sections";
    private string format_ArticlesList => """
    Generate a JSON response in this EXACT structure:
    {
    "Articles": [
        {
        "Id": <1>,
        "Title": <"Article Title">,
        "Abstract": < "**Key Point** - content with two trailing spaces.  **Key Point** - content with two trailing spaces.  **Key Point** - content with two trailing spaces.  ">,
        "Tags": <["tag1", "tag2", "tag3"]>
        },
        { article2 etc... }
    ],
    "AIMessage": <"Your message here">
    }

    **RULES:**
    - <> indicates a placeholder, replace with appropriate value, do not inlcude the brackets in responses.
    - Property names MUST be "Articles", "Id", "Title", "Abstract", "Tags", "AIMessage" (case-sensitive)
    1. **Id:** Start at 1 and increment for each article.
    2. **Title:** Clear and concise, describing the article's focus.
    3. **Abstract:** **KeyPoints**: one sentence briefly summarizing each points.
    4. **Tags:** list relevant keywords, 2-4 tags each article.
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
