using AutoMapper;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/articles")]
public class ArticleController(ILogger<ArticleController> _logger, IMapper _mapper,
    IArticleService _articleService,
    IArticleTagService _articleTagService)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<ArticleDto>> GetById(int id)
    {
        var article = await _articleService.GetByIdAsync(id);
        if (article == null)
            return ResultUtil.Error<ArticleDto>(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        var articleResponse = await _articleService.GetArticleDto(article);
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
        var pagedArticles = await _articleService.GetAllAsync(pageRequest);
        var articleDto = pagedArticles.Data.Where(x => x.ArticleType == ArticleTypes.Article);
        var articleList = await _articleService.GetArticleDtos(articleDto, pageRequest);
        var res = new Paged<ArticleDto>(pagedArticles.GetPageInfo(), articleList);
        return ResultUtil.Success(res);
    }

    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
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
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] ArticleEditRequest articleEditRequest)
        => ResultUtil.Success(await _articleService.EditArticleByRequest(articleEditRequest));
    

    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid article id");
        await _articleService.DeleteArticleById(id);
        return ResultUtil.Success(true);
    }

}
