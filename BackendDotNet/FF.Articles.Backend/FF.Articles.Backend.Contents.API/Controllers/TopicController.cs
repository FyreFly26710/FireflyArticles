namespace FF.Articles.Backend.Contents.API.Controllers;

[ApiController]
[Route("api/contents/topics")]
public class TopicController(ITopicService _topicService) : ControllerBase
{

    [HttpGet("{id}")]
    public async Task<ApiResponse<TopicDto>> GetById(long id, [FromQuery] TopicQueryRequest query)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Topic not found");
        TopicDto topicDto = await _topicService.GetTopicDto(topic, query);
        return ResultUtil.Success(topicDto);
    }

    /// <summary>
    /// Todo: Need to refactor
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<Paged<TopicDto>>> GetByPage([FromQuery] TopicQueryRequest pageRequest)
    {
        Paged<Topic> topics = await _topicService.GetAllAsync(pageRequest);
        Paged<TopicDto> res = new(topics.GetPageInfo());
        foreach (var topic in topics.Data)
        {
            // Do not include articles in topic list
            pageRequest.IncludeArticles = false;
            TopicDto topicDto = await _topicService.GetTopicDto(topic, pageRequest);
            res.Data.Add(topicDto);
        }
        return ResultUtil.Success(res);
    }

    [HttpGet("search")]
    public async Task<ApiResponse<TopicDto>> Search([FromQuery] string title, [FromQuery] string category)
    {
        var topicDto = await _topicService.GetTopicByTitleCategory(title, category);
        if (topicDto == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Topic not found");
        return ResultUtil.Success(topicDto);
    }

    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<long>> AddByRequest([FromBody] TopicAddRequest topicAddRequest)
    {
        //string title = topicAddRequest.Title;
        //var existTopic = await _topicService.GetTopicByTitle(title);
        //if (existTopic != null)
        //{
        //    return ResultUtil.Success(existTopic.Id);
        //}

        var topic = topicAddRequest.ToEntity();
        var userDto = UserUtil.GetUserFromHttpRequest(Request);
        topic.UserId = userDto.UserId;
        long topicId = await _topicService.CreateAsync(topic);
        return ResultUtil.Success(topicId);
    }

    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] TopicEditRequest topicEditRequest)
        => ResultUtil.Success(await _topicService.EditTopicByRequest(topicEditRequest));


    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    // Related Articles will not be deleted
    public async Task<ApiResponse<bool>> DeleteById(long id)
        => ResultUtil.Success(await _topicService.DeleteAsync(id));


}