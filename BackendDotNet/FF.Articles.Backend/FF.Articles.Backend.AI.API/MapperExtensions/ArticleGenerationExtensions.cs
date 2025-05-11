namespace FF.Articles.Backend.AI.API.MapperExtensions;

public static class ArticleGenerationExtensions
{
    public static ArticleApiUpsertRequest ToArticleApiUpsertRequest(this ContentRequest request, string content = "", long? parentArticleId = null)
    {
        return new ArticleApiUpsertRequest
        {
            Id = request.Id,
            Title = request.Title,
            Abstract = request.Abstract,
            Content = content,
            Tags = request.Tags,
            TopicId = request.TopicId,
            SortNumber = request.SortNumber,
            ArticleType = parentArticleId.HasValue ? "SubArticle" : "Article",
            ParentArticleId = parentArticleId,
        };
    }
}
