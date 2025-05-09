﻿using Asp.Versioning;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Route("api/contents/tags")]
public class TagController : TagControllerBase
{
    public TagController(Func<string, ITagService> tagService)
        : base(tagService, "v1")
    {
    }
}
