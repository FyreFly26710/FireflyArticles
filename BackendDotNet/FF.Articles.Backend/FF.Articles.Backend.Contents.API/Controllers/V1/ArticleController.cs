﻿using Asp.Versioning;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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
