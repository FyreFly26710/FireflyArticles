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

/// <summary>
/// Article Controller Base
/// </summary>
public abstract class ArticleControllerBase : ControllerBase
{
    private readonly IArticleService _articleService;
    /// <summary>
    /// Constructor
    /// </summary>
    public ArticleControllerBase(Func<string, IArticleService> articleService, string version)
    {
        _articleService = articleService(version);
    }

    #region REST API
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ArticleDto>> GetById(long id, [FromQuery] ArticleQueryRequest request)
    {
        var article = await _articleService.GetByIdAsync(id);
        if (article == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        var articleResponse = await _articleService.GetArticleDto(article, request);
        return ResultUtil.Success(articleResponse);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResponse<Paged<ArticleDto>>> GetByPage([FromQuery] ArticleQueryRequest pageRequest)
    {
        var pagedArticles = await _articleService.GetPagedArticlesByRequest(pageRequest);
        return ResultUtil.Success(pagedArticles);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="articleAddRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<long>> AddByRequest([FromBody] ArticleAddRequest articleAddRequest)
    {
        long articleId = await _articleService.CreateByRequest(articleAddRequest, UserUtil.GetUserId(Request));
        return ResultUtil.Success(articleId);
    }

    /// <summary>
    /// Create articles and return a dictionary of article id and title
    /// </summary>
    [HttpPost("batch")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<Dictionary<long, string>>> AddBatchByRequest([FromBody] List<ArticleAddRequest> articleAddRequests)
    {
        var result = await _articleService.CreateBatchAsync(articleAddRequests, UserUtil.GetUserId(Request));
        return ResultUtil.Success(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="articleEditRequest"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
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

    //[HttpPut("batch/content")]
    //[Authorize(Roles = UserConstant.ADMIN_ROLE)]
    //public async Task<ApiResponse<bool>> EditContentBatch([FromBody] Dictionary<long, string> batchEditConentRequests)
    //{
    //    var result = await _articleService.EditContentBatch(batchEditConentRequests);
    //    return ResultUtil.Success(result);
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteById(long id)
    {
        var result = await _articleService.DeleteArticleById(id);
        return ResultUtil.Success(result);
    }
    #endregion
}
