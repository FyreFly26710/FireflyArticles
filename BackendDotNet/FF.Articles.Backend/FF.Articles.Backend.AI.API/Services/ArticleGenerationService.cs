using System;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;
using FF.Articles.Backend.AI.Services;
using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using System.Text.Json;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.AI.API.Services.Stores;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using StackExchange.Redis;
using FF.Articles.Backend.AI.API.Models.Entities;

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
    private readonly IDatabase _redis;
    public ArticleGenerationService(IDeepSeekClient deepSeekClient, IContentsApiRemoteService contentsApiRemoteService, IConnectionMultiplexer redis)
    {
        _deepSeekClient = deepSeekClient;
        _contentsApiRemoteService = contentsApiRemoteService;
        _redis = redis.GetDatabase();
    }
    // Round 1
    public async Task<ArticlesAIResponseDto> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default)
    {
        var history = new AiGenHistory
        {
            Topic = request.Topic,
            ArticleCount = request.ArticleCount,
        };

        var chatRequest = new ChatRequest
        {
            Messages = {
                Message.NewSystemMessage(format_ArticlesList),
                Message.NewSystemMessage(system_ArticleList),
                Message.NewUserMessage(@$"Topic: {request.Topic}; ArticleCount: {request.ArticleCount}"),
            },
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.JsonObject }
        };
        Task<long> addTopicTask = _contentsApiRemoteService.AddTopicByTitleAsync(request.Topic);
        var response = await _deepSeekClient.ChatAsync(chatRequest, cancellationToken);
        var jsonContent = response?.Choices.First().Message?.Content ?? "";
        //var jsonContent = mock_article_list;

        // Extract content between first { and last }
        int firstBrace = jsonContent.IndexOf('{');
        int lastBrace = jsonContent.LastIndexOf('}');

        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            jsonContent = jsonContent.Substring(firstBrace, lastBrace - firstBrace + 1);
        }

        // await Task.Delay(1000);
        // Console.WriteLine(jsonContent);
        var articlesResponse = JsonSerializer.Deserialize<ArticlesAIResponseDto>(jsonContent);
        if (articlesResponse is null) throw new Exception("Failed to generate article");
        var topicId = await addTopicTask;
        articlesResponse.TopicId = topicId;
        history.AiResponse = jsonContent;
        history.TopicId = topicId;
        // await _redis.HashSetAsync($"history:{history.TopicId}", "topic", history.Topic);
        // await _redis.HashSetAsync($"history:{history.TopicId}", "articleCount", history.ArticleCount.ToString());
        // await _redis.HashSetAsync($"history:{history.TopicId}", "aiResponse", history.AiResponse);

        return articlesResponse;
    }

    // round 2
    public async Task<long> GenerateArticleContentAsync(ContentRequest request)
    {
        if (request.TopicId == 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Abstract))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");
        // var history = JsonSerializer.Deserialize<AiGenHistory>(await _redis.HashGetAsync($"history:{request.TopicId}", "topic"));
        //if (history is null) throw new Exception("Failed to generate article");
        // var topic = await _redis.HashGetAsync($"history:{request.TopicId}", "topic");
        // var articleCount = await _redis.HashGetAsync($"history:{request.TopicId}", "articleCount");
        // var aiResponse = await _redis.HashGetAsync($"history:{request.TopicId}", "aiResponse");

        var chatRequest = new ChatRequest
        {
            Messages = [
                //Message.NewSystemMessage(format_ArticlesList),
                //Message.NewSystemMessage(system_ArticleList),
                //Message.NewUserMessage(@$"Topic: {history.Topic}; ArticleCount: {history.ArticleCount}"),
                //Message.NewAssistantMessage(history.AiResponse),
                Message.NewSystemMessage(format_ArticleContent),
                Message.NewUserMessage(@$"
                Take a deep breath.
                Think step by step.
                You are a helpful assistant that generates an article based on the following information:
                Topic: {request.Topic}
                Title: {request.Title}
                Abstract: {request.Abstract}
                Tags: {string.Join(", ", request.Tags)}
                """),
            ],
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.Text }
        };

        var response = await _deepSeekClient.ChatAsync(chatRequest, new CancellationToken());

        var content = response?.Choices.First().Message?.Content ?? "";
        if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

        // Clean the content from outer markdown code fences
        content = content.Trim();
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
        "Abstract": "**Key Point** - content with two trailing spaces.  \n**Key Point** - content with two trailing spaces.  \n**Key Point** - content with two trailing spaces.  ",
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
    - **JSON Syntax:** Ensure all quotes, commas, and brackets are closed, do not include any code fences, your response should be a valid json object.
    """;
    private string format_ArticleContent => """
    For this round, you will be given a title, abstract, and tags.
    Respond ONLY with the raw markdown content following this exact structure:
    
        # Introduction
        Content...

        ## First Key Point (smaller point)
        Content...

        ## Second Key Point (larger point)
        Content...
        Content...
        Content...

        # Conclusion
        Content...
    
    Introduction and Conclusion contents should be 1 paragraphs.
    Each Key Points should have 1-3 paragraphs.

    DO NOT include: 
    - Title
    - Opening and closing markdown code fences
    """;

    private string mock_article_list = """
    {
        "Articles": [
            {
                "Id": 1,
                "Title": "Introduction to AI in Education: Basics and Potential",
                "Abstract": "**AI Overview** - Brief explanation of AI and its role in modern education.  \n**Benefits** - How AI can personalize learning and improve efficiency.  \n**Challenges** - Initial barriers like cost and teacher training.  ",
                "Tags": ["AI", "education", "technology"]
            },
            {
                "Id": 2,
                "Title": "AI-Powered Personalized Learning: Adaptive Systems",
                "Abstract": "**Adaptive Learning** - How AI tailors content to individual student needs.  \n**Case Studies** - Examples of platforms like Khan Academy using AI.  \n**Future Trends** - Predictive analytics for student performance.  ",
                "Tags": ["personalized learning", "AI", "adaptive systems"]
            },
            {
                "Id": 3,
                "Title": "Automated Grading and Feedback: Saving Time for Educators",
                "Abstract": "**Efficiency** - How AI reduces grading workload for teachers.  \n**Accuracy** - AI's role in providing consistent and unbiased feedback.  \n**Limitations** - Areas where human judgment is still essential.  ",
                "Tags": ["grading", "feedback", "AI tools"]
            },
            {
                "Id": 4,
                "Title": "AI Chatbots in Education: Virtual Tutors and Assistants",
                "Abstract": "**24/7 Support** - How chatbots provide instant help to students.  \n**Engagement** - Interactive learning through conversational AI.  \n**Implementation** - Best practices for integrating chatbots in classrooms.  ",
                "Tags": ["chatbots", "virtual tutors", "AI assistants"]
            },
            {
                "Id": 5,
                "Title": "Ethical Considerations of AI in Education",
                "Abstract": "**Data Privacy** - Concerns around student data collection and usage.  \n**Bias in AI** - How algorithms can perpetuate inequalities.  \n**Transparency** - The need for explainable AI in educational settings.  ",
                "Tags": ["ethics", "data privacy", "AI bias"]
            },
            {
                "Id": 6,
                "Title": "AI for Special Education: Supporting Diverse Learners",
                "Abstract": "**Accessibility** - AI tools for students with disabilities.  \n**Customization** - Tailoring learning experiences for neurodiverse students.  \n**Success Stories** - Real-world applications making a difference.  ",
                "Tags": ["special education", "accessibility", "inclusive learning"]
            },
            {
                "Id": 7,
                "Title": "The Future of AI in Higher Education",
                "Abstract": "**Research Assistance** - AI in academic research and paper writing.  \n**Administrative Automation** - Streamlining admissions and scheduling.  \n**Global Classrooms** - AI enabling cross-border education.  ",
                "Tags": ["higher education", "AI research", "automation"]
            },
            {
                "Id": 8,
                "Title": "Building AI Literacy: Preparing Students for an AI-Driven World",
                "Abstract": "**Curriculum Integration** - Teaching AI concepts in K-12.  \n**Critical Thinking** - Helping students understand AI limitations.  \n**Career Readiness** - Skills needed for future AI-centric jobs.  ",
                "Tags": ["AI literacy", "future skills", "education policy"]
            }
        ],
        "AIMessage": "Generated 8 articles covering AI in education from basics to advanced applications, including personalized learning, ethical considerations, special education support, and future trends. All articles follow the requested structure."
    }
    """;

}
