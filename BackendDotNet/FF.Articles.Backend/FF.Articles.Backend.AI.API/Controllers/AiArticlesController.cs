using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Mvc;
namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/[controller]")]
public class AiArticlesController(IArticleGenerationService articleGenerationService) : ControllerBase
{

    [HttpPost("generate-article")]
    public async Task<IActionResult> GenerateArticle(string topic, int aricleCount, CancellationToken cancellationToken)
    {
        var test = UserUtil.GetUserFromHttpRequest(Request);
        var article = await articleGenerationService.GenerateArticleListsAsync(topic, aricleCount, cancellationToken);
        return Ok(article);
    }

    [HttpPost("generate-article-content")]
    public async Task<IActionResult> GenerateArticleContent(int articleId, CancellationToken cancellationToken)
    {
        var content = await articleGenerationService.GenerateArticleContentAsync(articleId, cancellationToken);
        return Ok(content);
    }

}
