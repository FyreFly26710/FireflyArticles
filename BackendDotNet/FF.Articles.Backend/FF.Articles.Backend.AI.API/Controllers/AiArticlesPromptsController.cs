namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles-prompts")]
public class AiArticlesPromptsController(IContentsApiRemoteService _contentsApiRemoteService) : ControllerBase
{
    [HttpPost("generate-article-list")]
    public async Task<ApiResponse<List<MessageDto>>> GenerateArticleList([FromBody] ArticleListRequest request)
    {
        var topic = await _contentsApiRemoteService.GetTopicByTitleCategory(request.Topic, request.Category);
        var messages = new List<MessageDto>();
        if (topic == null)
        {
            messages = new List<MessageDto>
            {
                Message.System(Prompts.System_ArticleList(request)).ToMessageDto(),
                Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleList : request.UserPrompt).ToMessageDto()
            };
        }
        else
        {
            messages = new List<MessageDto>
            {
                Message.System(Prompts.System_RegenerateArticleList(request, topic)).ToMessageDto(),
                Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleList : request.UserPrompt).ToMessageDto()
            };
        }
        return ResultUtil.Success(messages);
    }

    [HttpPost("generate-article-content")]
    public ApiResponse<List<MessageDto>> GenerateArticleContent([FromBody] ContentRequest request)
    {
        var messages = new List<MessageDto>
        {
            Message.System(Prompts.System_ArticleContent(request)).ToMessageDto(),
            Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleContent : request.UserPrompt).ToMessageDto()
        };
        return ResultUtil.Success(messages);
    }
    [HttpPost("regenerate-article-content")]
    public async Task<ApiResponse<List<MessageDto>>> RegenerateArticleContent([FromBody] RegenerateArticleContentRequest request)
    {
        var article = await _contentsApiRemoteService.GetArticleById(request.ArticleId);
        // if not found, return false
        if (article == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Article not found");

        // if is sub article, Not supported yet
        if (article.ArticleType == ArticleTypes.SubArticle) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Sub article not supported");

        var isTopicArticle = article.ArticleType == ArticleTypes.TopicArticle;
        var topic = await _contentsApiRemoteService.GetTopicById(article.TopicId, isTopicArticle);
        if (topic == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Topic not found");
        var messages = new List<MessageDto>();
        // if is topic article
        if (isTopicArticle)
        {
            messages = new List<MessageDto>
            {
                Message.System(Prompts.System_TopicArticleContent(topic)).ToMessageDto(),
                Message.User(Prompts.User_TopicArticleContent).ToMessageDto()
            };
        }

        // if is article
        if (!isTopicArticle)
        {
            var contentRequest = article.ToContentRequest(topic, request.UserPrompt);
            messages = new List<MessageDto>
            {
                Message.System(Prompts.System_ArticleContent(contentRequest)).ToMessageDto(),
                Message.User(string.IsNullOrEmpty(request.UserPrompt) ? Prompts.User_ArticleContent : request.UserPrompt).ToMessageDto()
            };
        }
        return ResultUtil.Success(messages);
    }
}
