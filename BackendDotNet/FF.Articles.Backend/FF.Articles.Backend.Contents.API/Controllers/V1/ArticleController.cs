﻿using Asp.Versioning;
using AutoMapper;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.MapperExtensions.Articles;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FF.Articles.Backend.Contents.API.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Route("api/contents/articles")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;
    public ArticleController(Func<string, IArticleService> articleService)
    {
        _articleService = articleService("v1");
    }

    #region REST API
    [HttpGet("{id}")]
    public async Task<ApiResponse<ArticleDto>> GetById(int id, [FromQuery] ArticleQueryRequest request)
    {
        var article = await _articleService.GetByIdAsync(id);
        if (article == null)
            return ResultUtil.Error<ArticleDto>(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        var articleResponse = await _articleService.GetArticleDto(article, request);
        return ResultUtil.Success(articleResponse);
    }

    [HttpGet]
    public async Task<ApiResponse<Paged<ArticleDto>>> GetByPage([FromQuery] ArticleQueryRequest pageRequest)
    {
        var pagedArticles = await _articleService.GetArticlesByPageRequest(pageRequest);
        return ResultUtil.Success(pagedArticles);
    }

    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<int>> AddByRequest([FromBody] ArticleAddRequest articleAddRequest)
    {
        int articleId = await _articleService.CreateByRequest(articleAddRequest, UserUtil.GetUserId(Request));
        return ResultUtil.Success(articleId);
    }
    /// <summary>
    /// Create articles and return a dictionary of article id and title
    /// </summary>
    [HttpPut("batch")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<Dictionary<int, string>>> AddBatchByRequest([FromBody] List<ArticleAddRequest> articleAddRequests)
    {
        var result = await _articleService.CreateBatchAsync(articleAddRequests, UserUtil.GetUserId(Request));
        return ResultUtil.Success(result);
    }

    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] ArticleEditRequest articleEditRequest)
    {
        var result = await _articleService.EditArticleByRequest(articleEditRequest);
        return ResultUtil.Success(result);
    }

    [HttpPost("batch/content")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditContentBatch([FromBody] Dictionary<int, string> batchEditConentRequests)
    {
        var result = await _articleService.EditContentBatch(batchEditConentRequests);
        return ResultUtil.Success(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteById(int id)
    {
        var result = await _articleService.DeleteArticleById(id);
        return ResultUtil.Success(result);
    }
    #endregion
}
