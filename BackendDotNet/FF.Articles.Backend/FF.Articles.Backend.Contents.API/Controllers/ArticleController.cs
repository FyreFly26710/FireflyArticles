using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/article")]
public class ArticleController(IArticleService _articleService, ILogger<ArticleController> _logger, IMapper _mapper)
    : ControllerBase
{

    #region CRUD Operations
    [HttpPost("/add")]
    public async Task<ApiResponse<long>> AddArticle([FromBody] ArticleAddRequest articleAddRequest)
    {
        if (articleAddRequest == null)
            return ResultUtil.Error<long>(ErrorCode.PARAMS_ERROR);
        var article = _mapper.Map<Article>(articleAddRequest);
        var userDto = UserUtil.GetUserFromHttpRequest(Request);
        //article.UserId = User.GetUserId();
        //article.CreateTime = DateTime.Now;
        //article.UpdateTime = DateTime.Now;
        //article.IsHidden = 0;
        //article.SortNumber = 0;
        //await _articleService.AddAsync(article);
        return ResultUtil.Success(article.Id);
    }


    #endregion
}
