namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles")]
public class AiArticlesController(IArticleGenerationService articleGenerationService) : ControllerBase
{

    [HttpPost("generate-article-list")]
    public async Task<ApiResponse<string>> GenerateArticleList(ArticleListRequest request, CancellationToken cancellationToken)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var article = await articleGenerationService.GenerateArticleListsAsync(request, cancellationToken);
        return ResultUtil.Success<string>(article);
    }

    [HttpPost("generate-article-content")]
    public async Task<ApiResponse<long>> GenerateArticleContent(ContentRequest request)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);
        var articleId = await articleGenerationService.DispatchArticleGenerationAsync(request);
        return ResultUtil.Success<long>(articleId);
    }
    // [HttpPost("generate-article-content-batch")]
    // public async Task<IActionResult> BatchGenerateArticleContent(List<int> articleIds, CancellationToken cancellationToken)
    // {
    //     await articleGenerationService.BatchGenerateArticleContentAsync(articleIds, Request, cancellationToken);
    //     return Ok();
    // }

    [HttpPost("regenerate-article-content")]
    public async Task<ApiResponse<bool>> RegenerateArticleContent(long articleId)
    {
        var user = UserUtil.GetUserFromHttpRequest(Request);

        var result = await articleGenerationService.RegenerateArticleContentAsync(articleId);
        return ResultUtil.Success<bool>(result);
    }

}

