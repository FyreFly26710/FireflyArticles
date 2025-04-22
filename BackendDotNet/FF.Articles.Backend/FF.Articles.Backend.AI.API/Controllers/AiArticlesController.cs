using FF.AI.Common.Constants;
using FF.Articles.Backend.AI.API.Interfaces.Services;
using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.AI.API.Models.Dtos;
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
        request.Model = "deepseek-chat";
        request.Provider = ProviderList.DeepSeek;
        var article = await articleGenerationService.GenerateArticleListsAsync(request, cancellationToken);
        return ResultUtil.Success<ArticlesAIResponseDto>(article);
    }

    [HttpPost("generate-article-content")]
    public async Task<ApiResponse<long>> GenerateArticleContent(ContentRequest request)
    {
        request.Model = "deepseek-chat";
        request.Provider = ProviderList.DeepSeek;
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

