namespace FF.Articles.Backend.AI.API.Services;

public class ArticleGenerationService : IArticleGenerationService
{
  private readonly IAssistant _aiChatAssistant;
  private readonly IContentsApiRemoteService _contentsApiRemoteService;
  private readonly ILogger<ArticleGenerationService> _logger;
  // private readonly IDatabase _redis;
  public ArticleGenerationService(IAssistant aiChatAssistant, IContentsApiRemoteService contentsApiRemoteService, ILogger<ArticleGenerationService> logger, IRabbitMqPublisher rabbitMqPublisher)
  {
    _aiChatAssistant = aiChatAssistant;
    _contentsApiRemoteService = contentsApiRemoteService;
    _logger = logger;
    // _redis = redis.GetDatabase();
  }
  /// <summary>
  /// Round 1: Generate article lists
  /// Ai may not generate valid json response. Send the string let frontend parse it.
  /// </summary>
  public async Task<string> GenerateArticleListsAsync(ArticleListRequest request, CancellationToken cancellationToken = default)
  {
    return mockResponse();
    var model = getModel(request.Provider, false);
    var chatRequest = new ChatRequest
    {
      Model = model,
      Provider = request.Provider!,
      Messages = {
                Message.User(Prompts.User_ArticleList(request)),
            },
      Options = new ChatOptions() { Temperature = 0.5 }
      //Options = new ChatOptions() { ResponseFormat = ChatOptions.GetResponseFormat<ArticlesAIResponseDto>(), Temperature = 0.5 }
    };
    var topic = new TopicApiAddRequest
    {
      Title = request.Topic,
      Category = request.Category,
      Abstract = request.TopicAbstract
    };
    var topicId = await _contentsApiRemoteService.AddTopic(topic, AdminUsers.SYSTEM_ADMIN_DEEPSEEK);
    _logger.LogInformation("AI:{model};Begin to generate topic: {topic}; TopicId: {topicId}", model, request.Topic, topicId);
    var response = await _aiChatAssistant.ChatAsync(chatRequest, cancellationToken);
    var jsonContent = response?.Message?.Content ?? "";
    _logger.LogInformation("Request to generate topic: {topic}; Milliseconds taken : {time}; Tokens: {tokens}", request.Topic, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);

    var extractedJson = extractJson(jsonContent, topicId, request.Category);

    return extractedJson;
  }



  public async Task<string> GenerateArticleContentAsync(ContentRequest request)
  {
    return "test";
    if (request.TopicId == 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Abstract) || request.Id == null)
      throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");

    var model = getModel(request.Provider, false);
    var chatRequest = new ChatRequest
    {
      Model = model,
      Provider = request.Provider!,
      Messages = [Message.User(Prompts.User_ArticleContent(request))],
      Options = new ChatOptions() { Temperature = 0.5 }
    };
    if (request.UserPrompt != null)
    {
      chatRequest.Messages.Add(Message.User(request.UserPrompt));
    }
    _logger.LogInformation("AI: {model}. Begin to generate article: {title}", model, request.Title);
    var response = await _aiChatAssistant.ChatAsync(chatRequest, new CancellationToken());

    var content = response?.Message?.Content ?? "";
    if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate article");

    content = removeOuterCodeFences(content);
    _logger.LogInformation("Request to generate article: {title}; Milliseconds taken : {time}; Tokens: {tokens}", request.Title, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);

    return content;
  }

  public async Task<string> GenerateTopicContentAsync(TopicApiDto topic)
  {
    if (topic.TopicId == 0 || string.IsNullOrEmpty(topic.Title))
      throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid request");

    var model = getModel();
    var chatRequest = new ChatRequest
    {
      Model = model,
      Provider = ProviderList.DeepSeek,
      Messages = [
            Message.User(Prompts.User_TopicArticleContent(topic)),
            ],
      Options = new ChatOptions() { Temperature = 0.5 }
    };
    _logger.LogInformation("AI: {model}. Begin to generate topic article: {title}", model, topic.Title);
    var response = await _aiChatAssistant.ChatAsync(chatRequest, new CancellationToken());

    var content = response?.Message?.Content ?? "";
    if (string.IsNullOrEmpty(content)) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Failed to generate topic article");

    content = removeOuterCodeFences(content);
    _logger.LogInformation("Request to generate topic article: {title}; Milliseconds taken : {time}; Tokens: {tokens}", topic.Title, response?.ExtraInfo?.Duration, response?.ExtraInfo?.OutputTokens);

    return content;
  }


  #region Helper methods
  private string removeOuterCodeFences(string content)
  {
    if (string.IsNullOrWhiteSpace(content))
      return content;

    var lines = content.Split('\n').Select(l => l.TrimEnd('\r')).ToList();

    // Check for opening fence
    if (lines.Count >= 2 && lines[0].StartsWith("```") && lines[^1] == "```")
    {
      lines.RemoveAt(0);           // remove first line
      lines.RemoveAt(lines.Count - 1);  // remove last line
    }

    return string.Join("\n", lines).Trim();
  }

  private string getModel(string? provider = null, bool requireStructuredOutput = false)
  {
    if (provider is null || provider == ProviderList.DeepSeek)
    {
      return requireStructuredOutput ? "deepseek-chat" : "deepseek-reasoner";
    }
    else if (provider == ProviderList.Gemini)
    {
      return requireStructuredOutput ? "gemini-2.5-flash-preview-04-17" : "gemini-2.5-flash-preview-04-17";
    }
    else
    {
      throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid provider");
    }
  }
  private string extractJson(string jsonContent, long topicId, string category)
  {
    try
    {
      // Extract content between first { and last }
      int firstBrace = jsonContent.IndexOf('{');
      int lastBrace = jsonContent.LastIndexOf('}');

      if (firstBrace >= 0 && lastBrace > firstBrace && lastBrace < jsonContent.Length)
      {
        // Get the JSON content
        string extractedJson = jsonContent.Substring(firstBrace, lastBrace - firstBrace + 1);

        // Remove the closing brace to add our properties
        string jsonWithoutClosingBrace = extractedJson.Substring(0, extractedJson.Length - 1);

        // Append TopicId and Category and add the closing brace back
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
          "TopicId":"12345",
          "Category":"Web Development"
        }
        """;
}
