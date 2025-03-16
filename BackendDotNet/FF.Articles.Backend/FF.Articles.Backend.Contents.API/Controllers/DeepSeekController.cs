using System;
using FF.Articles.Backend.Contents.API.Services.AIServices;
using Microsoft.AspNetCore.Mvc;
namespace FF.Articles.Backend.Contents.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeepSeekController : ControllerBase
{
    private readonly ArticleGenerationService deepSeekService;

    public DeepSeekController(ArticleGenerationService deepSeekService)
    {
        this.deepSeekService = deepSeekService;
    }

    [HttpPost("generate-article")]
    public async Task<IActionResult> GenerateArticle(string topic, int aricleCount, CancellationToken cancellationToken)
    {
        var article = await deepSeekService.GenerateArticleListsAsync(topic, aricleCount, cancellationToken);
        return Ok(article);
    }

}
