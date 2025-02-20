using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class ArticleService(ContentsDbContext _context, ILogger<ArticleService> _logger, IMapper _mapper,
    IArticleTagService _articleTagService,
    IIdentityRemoteService _identityRemoteService
)
: CacheService<Article, ContentsDbContext>(_context, _logger), IArticleService
{
    public async Task<ArticleDto> GetArticleDto(Article article, bool includeUser = true)
    {
        var articleDto = _mapper.Map<ArticleDto>(article);
        if(includeUser) 
            articleDto.User = await _identityRemoteService.GetUserByIdAsync(article.UserId);
        Topic? topic = await _context.Set<Topic>().FindAsync(article.TopicId);
        articleDto.TopicTitle = topic?.Title ?? "Default Topic";
        articleDto.Tags = _articleTagService.GetArticleTags(articleDto.ArticleId);
        return articleDto;
    }
    public async Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, bool includeUser = true)
    {
        List<ArticleDto> articleDtos = new();
        foreach (var article in articles)
        {
            articleDtos.Add(await GetArticleDto(article, includeUser));
        }
        return articleDtos;
    }
    public async Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest)
    {
        var article = await this.GetByIdAsTrackingAsync(articleEditRequest.ArticleId);
        if (article == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Article not found");
        // update not null fields
        if (articleEditRequest.IsHidden != null) article.IsHidden = (int)articleEditRequest.IsHidden;
        if (articleEditRequest.Title != null) article.Title = articleEditRequest.Title;
        if (articleEditRequest.Content != null) article.Content = articleEditRequest.Content;
        if (articleEditRequest.Abstraction != null) article.Abstraction = articleEditRequest.Abstraction;
        //todo: check if topic exists
        if (articleEditRequest.TopicId != null) article.TopicId = (int)articleEditRequest.TopicId;
        if (articleEditRequest.SortNumber != null) article.SortNumber = (int)articleEditRequest.SortNumber;
        if (articleEditRequest.TagIds != null) await _articleTagService.EditArticleTags(article.Id, articleEditRequest.TagIds);
        await this.UpdateAsync(article);

        return true;
    }
    public async Task<bool> DeleteArticleById(int id)
    {
        if (id <= 0)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid article id");
        await this.DeleteAsync(id);
        await _articleTagService.RemoveArticleTags(id);
        return true;
    }
}

