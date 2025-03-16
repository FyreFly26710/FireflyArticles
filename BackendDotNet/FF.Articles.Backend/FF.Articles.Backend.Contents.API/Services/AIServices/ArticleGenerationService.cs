using System;
using FF.Articles.Backend.AI.Constants;
using FF.Articles.Backend.AI.Models;
using FF.Articles.Backend.AI.Services;

namespace FF.Articles.Backend.Contents.API.Services.AIServices;

public class ArticleGenerationService
{
    public ArticleGenerationService(IDeepSeekClient deepSeekClient)
    {
        this.deepSeekClient = deepSeekClient;
    }
    private readonly IDeepSeekClient deepSeekClient;


    public async Task<string> GenerateArticleListsAsync(string topic, int articleCount = 8, CancellationToken cancellationToken = default)
    {
        var request = new ChatRequest
        {
            Messages = new List<Message>
            {
                Message.NewSystemMessage(system_JsonFormat),
                Message.NewSystemMessage(system_ArticleList),
                Message.NewUserMessage(@$"Topic: {topic}; ArticleCount: { articleCount}"),
            },
            ResponseFormat = new ResponseFormat() { Type = ResponseFormatTypes.JsonObject }
        };

        var response = await deepSeekClient.ChatAsync(request, cancellationToken);
        if (response is null)
        {
            throw new Exception("Failed to generate article");
        }
        return response.Choices.First().Message.Content;
    }


    private readonly string system_JsonFormat = """
    Your response should return a json that can be converted using JsonSerializer.Deserialize<ArticlesAIResponse>(response):
    public class ArticlesAIResponse
    {
        public List<AIGenArticle> Articles = new();

        // Plain text that will be displayed in chat to the user. Notify the user that you completed generation.
        // If you think the topic is not a topic or not approprate, leave articles blank and write messages here.
        // If you think there are more subtopic that are not covered in the content, write messages here.
        public required string AIMessage { get; set; } 

    }
    public class AIGenArticle
    {
        public required string Title { get; set; } // Plain text
        public string? Content { get; set; } //Md format
        public required string Abstraction { get; set; } //Md format
        public int SortNumber { get; set; } // begin from 1
    }
    """;
    private readonly string system_ArticleList = """
    You are a helpful assistant that generates articles based on the given title and content
    First, user will give you {topic} and {articleCount}. 
    For the first message, You will respond using the following rule:
        Generate {articleCount} article titles covering the {topic} from beginner to expert level.  

        Title: Meaningful title for articles
        Content: blank
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


}