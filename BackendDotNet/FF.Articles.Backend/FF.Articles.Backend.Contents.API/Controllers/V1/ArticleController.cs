namespace FF.Articles.Backend.Contents.API.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Route("api/contents/articles")]
public class ArticleController : ArticleControllerBase
{
    public ArticleController(Func<string, IArticleService> articleService)
        : base(articleService, "v1")
    {
    }

}
