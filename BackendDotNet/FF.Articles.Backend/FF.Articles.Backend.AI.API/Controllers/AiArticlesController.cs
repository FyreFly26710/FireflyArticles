using FF.Articles.Backend.AI.API.Interfaces;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Models.AiDtos;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FF.Articles.Backend.AI.API.Controllers;

[ApiController]
[Route("api/ai/articles")]
public class AiArticlesController(IArticleGenerationService articleGenerationService) : ControllerBase
{

    [HttpPost("generate-article-list")]
    public async Task<ApiResponse<ArticlesAIResponseDto>> GenerateArticleList(ArticleListRequest request, CancellationToken cancellationToken)
    {
        var article = await articleGenerationService.GenerateArticleListsAsync(request, cancellationToken);
        return ResultUtil.Success<ArticlesAIResponseDto>(article);
    }

    [HttpPost("generate-article-content")]
    public async Task<ApiResponse<long>> GenerateArticleContent(ContentRequest request)
    {
        var content = await articleGenerationService.GenerateArticleContentAsync(request);
        return ResultUtil.Success<long>(content);
    }
    // [HttpPost("generate-article-content-batch")]
    // public async Task<IActionResult> BatchGenerateArticleContent(List<int> articleIds, CancellationToken cancellationToken)
    // {
    //     await articleGenerationService.BatchGenerateArticleContentAsync(articleIds, Request, cancellationToken);
    //     return Ok();
    // }



}

