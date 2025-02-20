using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/articles")]
public class ArticleController(ILogger<ArticleController> _logger, IMapper _mapper,
    IArticleService _articleService,
    ITopicService _topicService,
    IArticleTagService _articleTagService,
    IIdentityRemoteService _identityRemoteService)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<ArticleDto>> GetById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<ArticleDto>(ErrorCode.PARAMS_ERROR, "Invalid article id");
        var article = await _articleService.GetByIdAsync(id);
        if (article == null)
            return ResultUtil.Error<ArticleDto>(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        var articleResponse = _mapper.Map<ArticleDto>(article);
        articleResponse.User = await _identityRemoteService.GetUserByIdAsync(article.UserId);
        Topic? topic = await _topicService.GetByIdAsync(article.TopicId);
        articleResponse.TopicTitle = topic?.Title??"Topic Not Found";
        return ResultUtil.Success(articleResponse);
    }

    [HttpGet]
    public async Task<ApiResponse<Paged<ArticleDto>>> GetByPage([FromQuery]ArticlePageRequest pageRequest)
    {
        if (pageRequest == null || pageRequest.PageSize > 200)
            return ResultUtil.Error<Paged<ArticleDto>>(ErrorCode.PARAMS_ERROR, "Invalid page request");
        if(pageRequest.SortField == null)
        {
            pageRequest.SortField = "SortNumber";
        }
        var articles = await _articleService.GetAllAsync(pageRequest);
        var articleList = _mapper.Map<List<ArticleDto>>(articles.Data);
        foreach (var article in articleList)
        {
            if (pageRequest.IncludeUser)
            {
                article.User = await _identityRemoteService.GetUserByIdAsync(article.UserId);
            }
            Topic? topic = await _topicService.GetByIdAsync(article.TopicId);
            article.TopicTitle = topic?.Title ?? "Invalid topic";
            article.Tags = _articleTagService.GetArticleTags(article.ArticleId);
        }
        var res = new Paged<ArticleDto>(articles.GetPageInfo(), articleList);
        return ResultUtil.Success(res);
    }

    [HttpPut]
    public async Task<ApiResponse<int>> AddByRequest([FromBody] ArticleAddRequest articleAddRequest)
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

    [HttpPost]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] ArticleEditRequest articleEditRequest)
    {
        if (articleEditRequest == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR);

        var article = await _articleService.GetByIdAsTrackingAsync(articleEditRequest.ArticleId);
        if (article == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Article not found");

        // update not null fields
        if (articleEditRequest.IsHidden != null) article.IsHidden = (int)articleEditRequest.IsHidden;
        if (articleEditRequest.Title!=null) article.Title = articleEditRequest.Title;
        if (articleEditRequest.Content != null) article.Content = articleEditRequest.Content;
        if (articleEditRequest.Abstraction != null) article.Abstraction = articleEditRequest.Abstraction;
        //todo: check if topic exists
        if (articleEditRequest.TopicId!=null) article.TopicId = (int)articleEditRequest.TopicId;
        if (articleEditRequest.SortNumber != null) article.SortNumber = (int)articleEditRequest.SortNumber;
        if (articleEditRequest.TagIds != null) await _articleTagService.EditArticleTags(article.Id, articleEditRequest.TagIds);
        await _articleService.UpdateAsync(article);

        return ResultUtil.Success(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid article id");
        await _articleService.DeleteAsync(id);
        await _articleTagService.RemoveArticleTags(id);
        return ResultUtil.Success(true);
    }

}
