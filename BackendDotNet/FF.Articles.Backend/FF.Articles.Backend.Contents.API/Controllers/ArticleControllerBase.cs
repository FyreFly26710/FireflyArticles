using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers;

public abstract class ArticleControllerBase : ControllerBase
{
    private readonly IArticleService _articleService;
    public ArticleControllerBase(Func<string, IArticleService> articleService, string version)
    {
        _articleService = articleService(version);
    }

    #region REST API
    [HttpGet("{id}")]
    public async Task<ApiResponse<ArticleDto>> GetById(long id, [FromQuery] ArticleQueryRequest request)
    {
        var article = await _articleService.GetByIdAsync(id);
        if (article == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        var articleResponse = await _articleService.GetArticleDto(article, request);
        return ResultUtil.Success(articleResponse);
    }

    [HttpGet]
    public async Task<ApiResponse<Paged<ArticleDto>>> GetByPage([FromQuery] ArticleQueryRequest pageRequest)
    {
        if (pageRequest.PageSize > 30)
        {
            pageRequest.PageSize = 30;
        }

        var pagedArticles = await _articleService.GetPagedArticlesByRequest(pageRequest);
        return ResultUtil.Success(pagedArticles);
    }

    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<long>> AddByRequest([FromBody] ArticleAddRequest articleAddRequest)
    {
        long articleId = await _articleService.CreateByRequest(articleAddRequest, UserUtil.GetUserId(Request));
        return ResultUtil.Success(articleId);
    }

    [HttpPost("batch")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<Dictionary<long, string>>> AddBatchByRequest([FromBody] List<ArticleAddRequest> articleAddRequests)
    {
        var result = await _articleService.CreateBatchAsync(articleAddRequests, UserUtil.GetUserId(Request));
        return ResultUtil.Success(result);
    }

    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] ArticleEditRequest articleEditRequest)
    {
        var article = await _articleService.GetByIdAsync(articleEditRequest.ArticleId);
        if (article == null)
        {

            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        }
        else
        {
            await _articleService.EditArticleByRequest(articleEditRequest);
        }
        return ResultUtil.Success(true);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteById(long id)
    {
        var result = await _articleService.DeleteArticleById(id);
        return ResultUtil.Success(result);
    }
    #endregion
}
