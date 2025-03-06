using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Services;
public class ArticleService(ContentsDbContext _context, ILogger<ArticleService> _logger, IMapper _mapper,
    IArticleTagService _articleTagService,
    IIdentityRemoteService _identityRemoteService
)
: BaseService<Article, ContentsDbContext>(_context, _logger), IArticleService
{
    public async Task<ArticleDto> GetArticleDto(Article article) => await GetArticleDto(article, new ArticlePageRequest());

    public async Task<ArticleDto> GetArticleDto(Article article, ArticlePageRequest articleRequest)
    {
        var articleDto = _mapper.Map<ArticleDto>(article);
        if (articleRequest.IncludeUser)
            articleDto.User = await _identityRemoteService.GetUserByIdAsync(article.UserId);
        //only support 1 level sub articles
        // todo: remove ArticleType constraint and support more levels
        if (articleRequest.IncludeSubArticles && article.ArticleType == ArticleTypes.Article)
        {
            List<Article> subArticles = this.GetQueryable()
                .Where(x => x.ParentArticleId == articleDto.ArticleId
                    && x.ArticleType == ArticleTypes.SubArticle
                    //&& x.TopicId == articleDto.TopicId
                    )
                .OrderBy(x => x.SortNumber)
                .ToList();
            articleDto.SubArticles = await GetArticleDtos(subArticles, articleRequest);
        }
        if (!articleRequest.IncludeContent)
        {
            articleDto.Content = string.Empty;
        }
        Topic? topic = await _context.Set<Topic>().FindAsync(article.TopicId);
        articleDto.TopicTitle = topic?.Title ?? "Default Topic";
        articleDto.Tags = _articleTagService.GetArticleTags(articleDto.ArticleId);
        return articleDto;
    }
    public async Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticlePageRequest articleRequest)
    {
        List<ArticleDto> articleDtos = new();
        foreach (var article in articles)
        {
            articleDtos.Add(await GetArticleDto(article, articleRequest));
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
        if (articleEditRequest.TopicId != null) article.TopicId = (int)articleEditRequest.TopicId;
        if (articleEditRequest.ArticleType != null) article.ArticleType = articleEditRequest.ArticleType;
        if (articleEditRequest.ParentArticleId != null) article.ParentArticleId = (int)articleEditRequest.ParentArticleId;
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

    public async Task<IQueryable<Article>> ApplySearchQuery(IQueryable<Article> query, ArticlePageRequest pageRequest)
    {
        var keyword = pageRequest.Keyword;
        List<int>? topicIds = pageRequest.TopicIds;
        var tagIds = pageRequest.TagIds;

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(a => EF.Functions.Like(a.Title, $"%{keyword}%"));
        }

        if (topicIds != null && topicIds.Count > 0)
        {
            query = query.Where(a => topicIds.Contains(a.TopicId));
        }

        if (tagIds != null && tagIds.Count > 0)
        {
            query = from a in query
                    join at in _context.Set<ArticleTag>() on a.Id equals at.ArticleId
                    where tagIds.Contains(at.TagId)
                    select a;
        }
        return query;
    //   Executed DbCommand (36ms) [Parameters=[@__Format_1='%C%' (Size = 1000), @__topicIds_2='[1,2,3,4]' (Size = 4000), @__tagIds_3='[1,2,3,4,5]' (Size = 4000), @__p_4='0', @__p_5='20'], CommandType='Text', CommandTimeout='30']
    //   SELECT [a].[Id], [a].[Abstraction], [a].[ArticleType], [a].[Content], [a].[CreateTime], [a].[IsDelete], [a].[IsHidden], [a].[ParentArticleId], [a].[SortNumber], [a].[Title], [a].[TopicId], [a].[UpdateTime], [a].[UserId]
    //   FROM [Contents].[Article] AS [a]
    //   INNER JOIN [Contents].[ArticleTag] AS [a0] ON [a].[Id] = [a0].[ArticleId]
    //   WHERE [a].[IsDelete] = 0 AND [a].[ArticleType] = N'Article' AND [a].[Title] LIKE @__Format_1 AND [a].[TopicId] IN (
    //       SELECT [t].[value]
    //       FROM OPENJSON(@__topicIds_2) WITH ([value] int '$') AS [t]
    //   ) AND [a0].[TagId] IN (
    //       SELECT [t0].[value]
    //       FROM OPENJSON(@__tagIds_3) WITH ([value] int '$') AS [t0]
    //   )
    //   ORDER BY [a].[SortNumber]
    //   OFFSET @__p_4 ROWS FETCH NEXT @__p_5 ROWS ONLY

    }
}

