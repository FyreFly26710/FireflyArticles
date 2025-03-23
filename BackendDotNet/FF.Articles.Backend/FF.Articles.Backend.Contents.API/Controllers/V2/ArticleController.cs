using Asp.Versioning;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers.V2;

[ApiVersion(2.0)]
[ApiController]
[Route("api/contents/articles")]
public class ArticleController : ArticleControllerBase
{
    public ArticleController(Func<string, IArticleService> articleService)
        : base(articleService, "v2")
    {
    }

}
