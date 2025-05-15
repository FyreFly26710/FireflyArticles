namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles-prompts")]
public class AiArticlesPromptsController(IContentsApiRemoteService _contentsApiRemoteService) : ControllerBase
{
    [HttpPost("generate-article-list")]
    public ApiResponse<List<MessageDto>> GenerateArticleList([FromBody] ArticleListRequest request)
    {
        var messages = new List<MessageDto>
        {
            Message.System(Prompts.System_ArticleList(request)).ToMessageDto(),
            Message.User(request.UserPrompt ?? Prompts.User_ArticleList).ToMessageDto()
        };
        return ResultUtil.Success(messages);
    }
    [HttpPost("regenerate-article-list")]
    public async Task<ApiResponse<List<MessageDto>>> RegenerateArticleList([FromBody] ExistingArticleListRequest request)
    {
        var topic = await _contentsApiRemoteService.GetTopicById(request.TopicId, true);
        if (topic == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Topic not found");
        var messages = new List<MessageDto>
        {
            Message.System(Prompts.System_RegenerateArticleList(request, topic)).ToMessageDto(),
            Message.User(request.UserPrompt ?? Prompts.User_ArticleList).ToMessageDto()
        };
        return ResultUtil.Success(messages);
    }
    [HttpPost("generate-article-content")]
    public ApiResponse<List<MessageDto>> GenerateArticleContent([FromBody] ContentRequest request)
    {
        var messages = new List<MessageDto>
        {
            Message.System(Prompts.System_ArticleContent(request)).ToMessageDto(),
            Message.User(request.UserPrompt ?? Prompts.User_ArticleContent).ToMessageDto()
        };
        return ResultUtil.Success(messages);
    }
    [HttpPost("generate-topic-content/{topicId}")]
    public async Task<ApiResponse<List<MessageDto>>> GenerateTopicContent([FromRoute] long topicId)
    {
        var topic = await _contentsApiRemoteService.GetTopicById(topicId, true);
        if (topic == null) throw new ApiException(ErrorCode.SYSTEM_ERROR, "Topic not found");

        var messages = new List<MessageDto>
        {
            Message.System(Prompts.System_TopicArticleContent(topic)).ToMessageDto(),
            Message.User(Prompts.User_TopicArticleContent).ToMessageDto()
        };
        return ResultUtil.Success(messages);
    }
}
