using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.Controllers;

[ApiController]
[Route("api/contents/articles")]
public class ArticleController(IArticleService _articleService) : ControllerBase
{
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
        if (pageRequest.PageSize > 50)
        {
            pageRequest.PageSize = 50;
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

    // [HttpPost("batch")]
    // [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    // public async Task<ApiResponse<Dictionary<long, string>>> AddBatchByRequest([FromBody] List<ArticleAddRequest> articleAddRequests)
    // {
    //     var result = await _articleService.CreateBatchAsync(articleAddRequests, UserUtil.GetUserId(Request));
    //     return ResultUtil.Success(result);
    // }

    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] ArticleEditRequest articleEditRequest)
    {
        var article = await _articleService.GetByIdAsync(articleEditRequest.ArticleId);
        if (article == null)
        {
            var articleAddRequest = new ArticleAddRequest()
            {
                ArticleId = articleEditRequest.ArticleId,
                Title = articleEditRequest.Title ?? "",
                Abstract = articleEditRequest.Abstract ?? "",
                Content = articleEditRequest.Content ?? "",
                Tags = articleEditRequest.Tags ?? new List<string>(),
                TopicId = articleEditRequest.TopicId ?? 0,
                ArticleType = articleEditRequest.ArticleType ?? ArticleTypes.Article,
                ParentArticleId = articleEditRequest.ParentArticleId ?? 0,
                SortNumber = articleEditRequest.SortNumber ?? 0,
                IsHidden = articleEditRequest.IsHidden ?? 0,
            };
            await _articleService.CreateByRequest(articleAddRequest, UserUtil.GetUserId(Request));
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
}
