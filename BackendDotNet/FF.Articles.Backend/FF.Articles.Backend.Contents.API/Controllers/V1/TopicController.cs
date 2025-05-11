namespace FF.Articles.Backend.Contents.API.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Route("api/contents/topics")]
public class TopicController : TopicControllerBase
{
    public TopicController(Func<string, ITopicService> topicService)
        : base(topicService, "v1")
    {
    }

}