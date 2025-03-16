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

    // private ArticlesAIResponse? articlesAiResponse = null;
    // // AI generated article id (sort number) => Db article id
    // private Dictionary<int, int> idMap = new();
    // private string topic = "";
    // private bool isFirstRound = true;
    // Round 1
    public async Task<ArticlesAIResponse> GenerateArticleListsAsync(string topic, HttpRequest httpRequest, int articleCount = 8, CancellationToken cancellationToken = default)
    {
        var user = UserUtil.GetUserFromHttpRequest(httpRequest);
        var userArticleState = _userArticleStateStore.GetOrAddState(user.UserId);

        var request = new ChatRequest
        {
            Messages = new(){
                Message.NewSystemMessage(format_ArticlesList),
                Message.NewSystemMessage(system_ArticleList),
                Message.NewUserMessage(@$"Topic: {topic}; ArticleCount: {articleCount}"),
            },
            Stream = false
        };
        request.ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.JsonObject };

        var response = await _deepSeekClient.ChatAsync(request, cancellationToken);
        var jsonContent = response?.Choices.First().Message?.Content ?? "";
        userArticleState.ArticlesAIResponse = JsonSerializer.Deserialize<ArticlesAIResponse>(jsonContent);
        if (userArticleState.ArticlesAIResponse is null)
        {
            throw new Exception("Failed to generate article");
        }
        userArticleState.Topic = topic;
        // Insert topic
        userArticleState.TopicId = await _contentsApiRemoteService.AddTopicByTitleAsync(userArticleState.Topic, httpRequest);
        var articleApiRequests = userArticleState.ArticlesAIResponse!.Articles.Select(article => article.ToArticleApiRequest(userArticleState.TopicId)).ToList();
        var idTitleMap = await _contentsApiRemoteService.AddBatchArticlesAsync(articleApiRequests, httpRequest);
        userArticleState.IdMap = userArticleState.ArticlesAIResponse.Articles
            .ToDictionary(
                aiArticle => aiArticle.Id,
                aiArticle => idTitleMap
                    .First(kvp => kvp.Value == aiArticle.Title)
                    .Key
            );
        userArticleState.IsFirstRound = false;
        return userArticleState.ArticlesAIResponse;
    }

    // round 2
    public async Task<string> GenerateArticleContentAsync(int articleId, HttpRequest httpRequest, CancellationToken cancellationToken = default)
    {
        var user = UserUtil.GetUserFromHttpRequest(httpRequest);
        var userArticleState = _userArticleStateStore.GetOrAddState(user.UserId);
        if (userArticleState.IsFirstRound)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Please generate article lists first");
        }

        var selectedArticle = userArticleState.ArticlesAIResponse?.Articles.FirstOrDefault(a => a.Id == articleId);
        if (selectedArticle is null)
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");
        }
        var request = new ChatRequest
        {
            Messages = new(){
                // Todo: Add history messages?
                Message.NewUserMessage(user_ArticleContent(selectedArticle)),
                Message.NewSystemMessage(format_ArticleContent),
            },
        };
        request.ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.Text };

        var response = await _deepSeekClient.ChatAsync(request, cancellationToken);

        var content = response?.Choices.First().Message?.Content ?? "";
        if (string.IsNullOrEmpty(content))
        {
            throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");
        }

        // Update article
        var dbArticleId = userArticleState.IdMap[articleId];
        var editRequest = new ArticleApiEditRequest
        {
            ArticleId = dbArticleId,
            Content = content,
            // Todo: Add tags
            //Tags = tags,
        };
        await _contentsApiRemoteService.EditArticleByRequest(editRequest, httpRequest);
        return content;
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
        Abstraction:
            - Provide 2-3 key points, each in a single concise sentence. 
            - It does not need to be proper full sentences. Just key words and very brief explanation.
     
        Each title should explore a distinct subtopic or angle related to the main topic.
        Do not write too much articles about beginner levels.
        Respond each title in MD format.
     
        **Example Output Format**:
        Title: Rapid HTML Enhancement & Best Practices
        Abstraction:
        Semantic HTML - Using elements that convey meaning for better accessibility and SEO.
        Metadata & SEO - `<meta> tags`, `<title>`, how HTML influences search engines.
        Accessibility Considerations - `aria-*` attributes, proper use of headings and labels.
    """;


    private string user_ArticleContent(AIGenArticle article) => @$"
    You are a helpful assistant that generates articles based on the title: {article.Title} and key points: {article.Abstraction}  
    Formatting Requirements:
    - Strictly use pure markdown syntax ONLY
    - Omit the article title completely
    - Never use ```markdown code blocks
    - Maintain proper heading hierarchy (H2 for sections)
    - No markdown formatting in introduction/conclusion

    Article Structure:
    1. **Introduction** (H2 heading)
    - 1-2 paragraphs establishing context
    - Clearly state article purpose

    2. **Core Sections** (3-5 H2 sections)
    - Convert each key points from above to H2 heading
    - 1-3 paragraphs per section
    - Include examples/data where applicable
    - Use bullet points for lists where appropriate

    3. **Conclusion** (H2 heading)
    - Summarize key insights
    - Highlight practical applications
    - Avoid introducing new information

    Content Generation Guidelines:
    - Write in professional but accessible tone
    - Maintain academic rigor without jargon
    - Ensure logical flow between sections
    - All content must be original analysis
    - Keep paragraphs under 6 sentences";
    private string format_ArticlesList => """
    Generate a JSON response in this EXACT structure:
    {
    "Articles": [
        {
        "Id": 1,
        "Title": "Article Title",
        "Abstraction": 
            "**Key Point** content with two trailing spaces.  "
            "**Key Point** content with two trailing spaces.  "
            "**Key Point** content with two trailing spaces.  "
        }
    ],
    "AIMessage": "Your message here"
    }

    **RULES:**
    - Property names MUST be "Articles", "Id", "Title", "Abstraction", "AIMessage" (case-sensitive)
    - "Id" MUST start at 1 and increment
    - Escape line breaks with \\n (double backslash)
    1. **Id Field:** Start at 1 and increment for each article.
    2. **Title:** Clear and concise, describing the article's focus.
    3. **Abstraction:** Use **Markdown** (bold, lists, etc.) to structure content. Split ideas with `\n`.
    4. **AIMessage:** 
    - If the topic is valid: List key subtopics covered and confirm completion.
    - If invalid/off-topic: Leave `Articles` empty and explain why in `AIMessage`.
    5. **JSON Syntax:** Ensure all quotes, commas, and brackets are closed. Escape `\n` and `\"` properly.
    """;
    private string format_ArticleContent => """
    Respond ONLY with the raw markdown content following this exact structure:
    # Introduction
    [Content...]

    ## [First Key Point]
    [Content...]

    ## [Second Key Point] 
    [Content...]

    # Conclusion
    [Content...]

    DO NOT include: 
    - Title
    - Markdown code fences
    - Any text outside the specified structure
    """;
}
