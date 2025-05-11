namespace FF.Articles.Backend.Contents.API.Controllers.V2;

[ApiVersion(2.0)]
[ApiController]
[Route("api/contents/topics")]
public class TopicController : TopicControllerBase
{
    public TopicController(Func<string, ITopicService> topicService)
        : base(topicService, "v2")
    {
    }

}