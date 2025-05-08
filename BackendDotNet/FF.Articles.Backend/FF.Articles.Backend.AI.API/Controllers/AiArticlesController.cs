using FF.AI.Common.Constants;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Models.Dtos;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Mvc;
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



}

