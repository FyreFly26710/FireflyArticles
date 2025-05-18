namespace FF.Articles.Backend.AI.API.Services;

public class ArticleGenerationService(
  IAssistant _aiChatAssistant,
  IContentsApiRemoteService _contentsApiRemoteService,
  ILogger<ArticleGenerationService> _logger
// IRabbitMqPublisher _rabbitMqPublisher
) : IArticleGenerationService
{
  private ChatRequest GetChatRequest(List<Message> messages, string model, string provider) =>
    new ChatRequest
    {
      Model = model,
      Provider = provider,
      Messages = messages,
      Options = new ChatOptions() { Temperature = 0.5 }
    };
  private async Task<string> ChatAsync(ChatRequest request, string session)
  {
    _logger.LogInformation("AI:{model} begin generate {session}", request.Model, session);
    var response = await _aiChatAssistant.ChatAsync(request, new CancellationToken());
    if (response is null || response.Message is null)
      throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

    _logger.LogInformation("AI:{model} end generate {session}; Milliseconds taken : {time}; Tokens: {tokens}", request.Model, session, response.ExtraInfo.Duration, response.ExtraInfo.OutputTokens + response.ExtraInfo.InputTokens);
    var jsonContent = response.Message.Content;
    if (string.IsNullOrEmpty(jsonContent))
      jsonContent = "Ai generate blank content";

    return jsonContent;
  }
  public async Task<string> GenerateArticleListsAsync(ArticleListRequest request)
  {
    var messages = new List<Message>
        {
          Message.System(Prompts.System_ArticleList(request)),
          Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleList : request.UserPrompt)
        };
    var chatRequest = GetChatRequest(messages, getModel(request.Provider, false), request.Provider!);
    var topic = new TopicApiAddRequest
    {
      Title = request.Topic,
      Category = request.Category,
      Abstract = request.TopicAbstract
    };
    var topicId = await _contentsApiRemoteService.AddTopic(topic, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);
    var jsonContent = await ChatAsync(chatRequest, $"Generate article list for topic: {request.Topic}");
    var extractedJson = extractJson(jsonContent, topicId, request.Category);

    return extractedJson;
  }

  public async Task<string> RegenerateArticleListAsync(ArticleListRequest request, TopicApiDto topic)
  {
    var messages = new List<Message>
        {
          Message.System(Prompts.System_RegenerateArticleList(request, topic)),
          Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleList : request.UserPrompt)
        };
    var chatRequest = GetChatRequest(messages, getModel(), ProviderList.DeepSeek);
    var jsonContent = await ChatAsync(chatRequest, $"Regenerate article list for topic: {topic.Title}");
    var extractedJson = extractJson(jsonContent, topic.TopicId, request.Category);

    return extractedJson;
  }

  public async Task<string> GenerateArticleContentAsync(ContentRequest request)
  {
    if (request.TopicId == 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Abstract) || request.Id == null)
      throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");

    var messages = new List<Message>
        {
          Message.System(Prompts.System_ArticleContent(request)),
          Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleContent : request.UserPrompt)
        };
    var chatRequest = GetChatRequest(messages, getModel(request.Provider, false), request.Provider!);
    var content = await ChatAsync(chatRequest, $"Generate article: {request.Title}");
    return removeOuterCodeFences(content);
  }

  public async Task<string> GenerateTopicContentAsync(TopicApiDto topic)
  {
    if (topic.TopicId == 0 || string.IsNullOrEmpty(topic.Title))
      throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");

    var messages = new List<Message>
        {
          Message.System(Prompts.System_TopicArticleContent(topic)),
          Message.User(Prompts.User_TopicArticleContent)
        };
    var chatRequest = GetChatRequest(messages, getModel(), ProviderList.DeepSeek);
    var content = await ChatAsync(chatRequest, $"Generate topic article: {topic.Title}");
    return removeOuterCodeFences(content);
  }


  #region Helper methods
  private string removeOuterCodeFences(string content)
  {
    if (string.IsNullOrWhiteSpace(content))
      return content;
    var lines = content.Split('\n').Select(l => l.TrimEnd('\r')).ToList();
    if (lines.Count >= 2 && lines[0].StartsWith("```markdown") && lines[^1] == "```")
    {
      lines.RemoveAt(0);
      lines.RemoveAt(lines.Count - 1);
    }
    return string.Join("\n", lines).Trim();
  }

  private string getModel(string? provider = null, bool requireStructuredOutput = false)
  {
    if (provider is null || provider == ProviderList.DeepSeek)
      return requireStructuredOutput ? "deepseek-chat" : "deepseek-reasoner";
    else if (provider == ProviderList.Gemini)
      return requireStructuredOutput ? "gemini-2.5-flash-preview-04-17" : "gemini-2.5-flash-preview-04-17";
    else
      throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid provider");
  }
  private string extractJson(string jsonContent, long topicId, string category)
  {
    try
    {
      int firstBrace = jsonContent.IndexOf('{');
      int lastBrace = jsonContent.LastIndexOf('}');

      if (firstBrace >= 0 && lastBrace > firstBrace && lastBrace < jsonContent.Length)
      {
        string extractedJson = jsonContent.Substring(firstBrace, lastBrace - firstBrace + 1);
        string jsonWithoutClosingBrace = extractedJson.Substring(0, extractedJson.Length - 1);
        jsonContent = jsonWithoutClosingBrace +
                    $", \"TopicId\": {topicId}, \"Category\": \"{category}\"" +
                    "}";
      }
      else
      {
        jsonContent += $"\"TopicId\": {topicId}, \"Category\": \"{category}\", \"ErrorMessage\": \"Failed to generate articles properly.\"";
      }
    }
    catch (Exception ex)
    {
      jsonContent += $"\"TopicId\": {topicId}, \"Category\": \"{category}\", \"ErrorMessage\": \"Error generating articles: {ex.Message}\"";
    }
    return jsonContent;
  }
  #endregion
  private string mockResponse() =>
    """
        {
          "Articles": [
            {
              "SortNumber": 1,
              "Title": "Getting Started with React: Building Your First Component",
              "Abstract": "Explore the foundational concepts of React, including JSX syntax and how to create functional components. This article provides a simple entry point into building user interfaces with React's declarative approach.",
              "Tags": [
                "Beginner",
                "Components",
                "JavaScript",
                "Overview",
                "Conversational"
              ]
            },
            {
              "SortNumber": 2,
              "Title": "Managing Data in React: Understanding State and Props",
              "Abstract": "Dive into the core mechanisms for data flow in React applications. Learn how state manages component-specific data and how props facilitate communication between parent and child components, enabling dynamic UIs.",
              "Tags": [
                "Beginner",
                "State Management",
                "JavaScript",
                "Technical",
                "Overview"
              ]
            },
            {
              "SortNumber": 3,
              "Title": "Mastering React Hooks: useState and useEffect Explained",
              "Abstract": "Understand how React Hooks simplify state management and side effects in functional components. This article covers the fundamental useState for adding state and useEffect for handling lifecycle-like behavior.",
              "Tags": [
                "Advanced",
                "Hooks",
                "JavaScript",
                "Technical",
                "Deep-dive"
              ]
            },
            {
              "SortNumber": 4,
              "Title": "Navigating Your React App: An Introduction to React Router",
              "Abstract": "Learn how to implement client-side routing in your single-page React applications using a popular library like React Router. Discover how to define routes and navigate between different views seamlessly.",
              "Tags": [
                "Beginner",
                "Routing",
                "JavaScript",
                "Overview",
                "Technical"
              ]
            },
            {
              "SortNumber": 5,
              "Title": "Building Complex UIs: Component Composition in React",
              "Abstract": "Explore how combining smaller, reusable components is key to building scalable and maintainable React applications. Learn patterns for composing components to create rich and interactive user interfaces efficiently.",
              "Tags": [
                "General",
                "Component Design",
                "JavaScript",
                "Best-practices",
                "Technical"
              ]
            }
          ],
          "AiMessage": "The topic 'React in Category: Web Development' is valid. The articles cover the following key subtopics: Introduction to Components and JSX, State and Props, React Hooks (useState, useEffect), Client-Side Routing, and Component Composition. The generation of 5 articles is complete.",
          "TopicId":"100",
          "Category":"Web Development"
        }
        """;
}
