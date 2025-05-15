namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles")]
public class AiArticlesController(IArticleGenerationService _articleGenerationService,
    IContentsApiRemoteService _contentsApiRemoteService,
    IRabbitMqPublisher _rabbitMqPublisher) : ControllerBase
{
    [HttpPost("generate-article-list")]
    public async Task<ApiResponse<string>> GenerateArticleList([FromBody] ArticleListRequest request)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var article = await _articleGenerationService.GenerateArticleListsAsync(request);
        return ResultUtil.Success<string>(article);
    }
    [HttpPost("regenerate-article-list")]
    public async Task<ApiResponse<string>> RegenerateArticleList([FromBody] ExistingArticleListRequest request)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var topic = await _contentsApiRemoteService.GetTopicById(request.TopicId, true);
        if (topic == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Topic not found");
        var article = await _articleGenerationService.RegenerateArticleListAsync(request, topic);
        return ResultUtil.Success<string>(article);

    }

    [HttpPost("generate-article-content")]
    public ApiResponse<long> GenerateArticleContent([FromBody] ContentRequest request)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var article = request.ToArticleApiUpsertRequest("Generating content...");
        long id = EntityUtil.GenerateSnowflakeId();
        request.Id = id;
        article.ArticleId = id;
        article.UserId = AdminUsers.SYSTEM_ADMIN_DEEPSEEK.UserId;
        _rabbitMqPublisher.Publish(QueueList.AddArticleQueue, article);
        _rabbitMqPublisher.Publish(QueueList.GenerateArticleQueue, request);

        return ResultUtil.Success<long>(id);
    }

    [HttpPost("regenerate-article-content")]
    public async Task<ApiResponse<bool>> RegenerateArticleContent([FromBody] TopicArticleContentRequest request)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var article = await _contentsApiRemoteService.GetArticleById(request.ArticleId);
        // if not found, return false
        if (article == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Article not found");

        // if is sub article, Not supported yet
        if (article.ArticleType == ArticleTypes.SubArticle) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Sub article not supported");

        var isTopicArticle = article.ArticleType == ArticleTypes.TopicArticle;
        var topic = await _contentsApiRemoteService.GetTopicById(article.TopicId, isTopicArticle);
        if (topic == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Topic not found");
        await _contentsApiRemoteService.EditArticleContentAsync(article.ArticleId, "Generating content...");

        // if is topic article
        if (isTopicArticle)
        {
            // topic includes article
            _rabbitMqPublisher.Publish(QueueList.GenerateTopicArticleQueue, topic);
            return ResultUtil.Success<bool>(true);
        }

        // if is article
        if (!isTopicArticle)
        {
            // topic does not include article
            var contentRequest = article.ToContentRequest(topic, request.UserPrompt);
            _rabbitMqPublisher.Publish(QueueList.GenerateArticleQueue, contentRequest);
            return ResultUtil.Success<bool>(true);
        }

        return ResultUtil.Success<bool>(true);
    }

}

