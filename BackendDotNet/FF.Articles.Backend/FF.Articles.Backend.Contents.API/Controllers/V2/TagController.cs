namespace FF.Articles.Backend.Contents.API.Controllers.V2;

[ApiVersion(2.0)]
[ApiController]
[Route("api/contents/tags")]
public class TagController : TagControllerBase
{
    public TagController(Func<string, ITagService> tagService)
        : base(tagService, "v2")
    {
    }
}
