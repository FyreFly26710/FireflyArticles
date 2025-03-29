using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Mvc;
namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles")]
public class AiArticlesController(IArticleGenerationService articleGenerationService) : ControllerBase
{

    [HttpPost("generate-article")]
    public async Task<IActionResult> GenerateArticle(string topic, int aricleCount, CancellationToken cancellationToken)
    {
        var article = await articleGenerationService.GenerateArticleListsAsync(topic, Request, aricleCount, cancellationToken);
        return Ok(article);
    }

    [HttpPost("generate-article-content")]
    public async Task<IActionResult> GenerateArticleContent(int articleId, CancellationToken cancellationToken)
    {
        var content = await articleGenerationService.GenerateArticleContentAsync(articleId, Request, cancellationToken);
        return Ok(content);
    }
    [HttpPost("generate-article-content-batch")]
    public async Task<IActionResult> BatchGenerateArticleContent(List<int> articleIds, CancellationToken cancellationToken)
    {
        await articleGenerationService.BatchGenerateArticleContentAsync(articleIds, Request, cancellationToken);
        return Ok();
    }

}
