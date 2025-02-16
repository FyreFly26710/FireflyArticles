using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Requests;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Responses;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/article")]
public class ArticleController(ILogger<ArticleController> _logger, IMapper _mapper,
    IArticleService _articleService,
    ITopicService _topicService,
    IArticleTagService _articleTagService,
    IIdentityRemoteService _identityRemoteService)
    : ControllerBase
{
    /// <summary>
    /// Get article response by id
    /// </summary>
    [HttpGet("get/{id}")]
    public async Task<ApiResponse<ArticleResponse>> GetById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<ArticleResponse>(ErrorCode.PARAMS_ERROR, "Invalid article id");
        var article = await _articleService.GetByIdAsync(id);
        if (article == null)
            return ResultUtil.Error<ArticleResponse>(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        var articleResponse = _mapper.Map<ArticleResponse>(article);
        articleResponse.User = await _identityRemoteService.GetUserByIdAsync(article.UserId);
        Topic? topic = await _topicService.GetByIdAsync(article.TopicId);
        articleResponse.TopicTitle = topic?.Title??"Topic Not Found";
        return ResultUtil.Success(articleResponse);
    }

    /// <summary>
    /// Get article response by page
    /// </summary>
    [HttpPost("get-page")]
    public async Task<ApiResponse<PageResponse<ArticleResponse>>> GetByPage(PageRequest pageRequest)
    {
        if (pageRequest == null || pageRequest.PageSize > 200)
            return ResultUtil.Error<PageResponse<ArticleResponse>>(ErrorCode.PARAMS_ERROR, "Invalid page request");
        if(pageRequest.SortField == null)
        {
            pageRequest.SortField = "SortNumber";
        }
        var articles = await _articleService.GetAllAsync(pageRequest);
        var articleList = _mapper.Map<List<ArticleResponse>>(articles.Data);
        // to do: improve get user logic
        foreach (var article in articleList)
        {
            article.User = await _identityRemoteService.GetUserByIdAsync(article.UserId);
            Topic? topic = await _topicService.GetByIdAsync(article.TopicId);
            article.TopicTitle = topic?.Title ?? "Invalid topic";
        }
        var res = new PageResponse<ArticleResponse>()
        {
            Data = articleList,
            PageIndex = articles.PageIndex,
            PageSize = articles.PageSize,
            RecordCount = articles.RecordCount
        };
        return ResultUtil.Success(res);
    }




    #region DB Modification
    /// <summary>
    /// Add article
    /// </summary>
    [HttpPost("add")]
    public async Task<ApiResponse<int>> AddArticle([FromBody] ArticleAddRequest articleAddRequest)
    {
        if (articleAddRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);
        var article = _mapper.Map<Article>(articleAddRequest);
        var userDto = UserUtil.GetUserFromHttpRequest(Request);
        article.UserId = userDto.UserId;
        //todo: check if topic exists
        int articleId = await _articleService.CreateAsync(article);
        await _articleTagService.EditArticleTags(articleId, articleAddRequest.TagIds);

        return ResultUtil.Success(article.Id);
    }
    /// <summary>
    /// Edit article
    /// </summary>
    /// <param name="articleEditRequest"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    public async Task<ApiResponse<int>> EditArticle([FromBody] ArticleEditRequest articleEditRequest)
    {
        if (articleEditRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);

        var article = await _articleService.GetByIdAsTrackingAsync(articleEditRequest.ArticleId);
        if (article == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR, "Article not found");

        if (articleEditRequest.IsHidden != 0) article.IsHidden = articleEditRequest.IsHidden;
        if (!string.IsNullOrWhiteSpace(articleEditRequest.Title)) article.Title = articleEditRequest.Title;
        if (!string.IsNullOrWhiteSpace(articleEditRequest.Content)) article.Content = articleEditRequest.Content;
        if (!string.IsNullOrWhiteSpace(articleEditRequest.Abstraction)) article.Abstraction = articleEditRequest.Abstraction;
        //todo: check if topic exists
        if (articleEditRequest.TopicId > 0) article.TopicId = articleEditRequest.TopicId;
        if (articleEditRequest.SortNumber > 0) article.SortNumber = articleEditRequest.SortNumber;
        await _articleTagService.EditArticleTags(article.Id, articleEditRequest.TagIds);
        await _articleService.UpdateAsync(article);

        return ResultUtil.Success(article.Id);
    }
    /// <summary>
    /// Delete article
    /// </summary>
    /// <param name="deleteByIdRequest"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    public async Task<ApiResponse<bool>> DeleteArticle([FromBody] DeleteByIdRequest deleteByIdRequest)
    {
        if (deleteByIdRequest.Id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid article id");
        await _articleService.DeleteAsync(deleteByIdRequest.Id);
        await _articleTagService.RemoveArticleTags(deleteByIdRequest.Id);
        return ResultUtil.Success(true);
    }

    #endregion
}
