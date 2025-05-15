namespace FF.Articles.Backend.AI.API.MapperExtensions;

public static class ArticleGenerationExtensions
{
    public static ArticleApiUpsertRequest ToArticleApiUpsertRequest(this ContentRequest request, string content = "", long? parentArticleId = null)
    {
        return new ArticleApiUpsertRequest
        {
            ArticleId = request.Id,
            Title = request.Title,
            Abstract = request.Abstract,
            Content = content,
            Tags = request.Tags,
            TopicId = request.TopicId,
            SortNumber = request.SortNumber,
            ArticleType = parentArticleId.HasValue ? ArticleTypes.SubArticle : ArticleTypes.Article,
            ParentArticleId = parentArticleId,
        };
    }
    public static ArticleApiUpsertRequest ToArticleApiUpsertRequest(this TopicApiDto request, string content = "")
    {
        return new ArticleApiUpsertRequest
        {
            ArticleId = request.TopicId,
            Title = request.Title,
            Abstract = request.Abstract,
            Content = content,
            Tags = new List<string>(),
            TopicId = request.TopicId,
            SortNumber = 0,
            ArticleType = ArticleTypes.TopicArticle,
        };
    }
    public static ContentRequest ToContentRequest(this ArticleApiDto dto, TopicApiDto topic, string? userPrompt = null)
    {
        return new ContentRequest
        {
            Id = dto.ArticleId,
            TopicId = topic.TopicId,
            Title = dto.Title,
            Abstract = dto.Abstract,
            Topic = topic.Title,
            TopicAbstract = topic.Abstract,
            Category = topic.Category,
            Tags = dto.Tags,
            SortNumber = dto.SortNumber,
            UserPrompt = userPrompt,
        };
    }
    public static MessageDto ToMessageDto(this Message message)
    {
        return new MessageDto
        {
            Content = message.Content,
            Role = message.Role
        };
    }
}