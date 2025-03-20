using AutoMapper;
using Azure.Core;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.MapperExtensions.Articles;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Services.V2;
public class ArticleService : BaseService<Article, ContentsDbContext>, IArticleService
{
    private readonly IArticleTagService _articleTagService;
    private readonly IIdentityRemoteService _identityRemoteService;
    private readonly ITopicRepository _topicRepository;
    private readonly IContentsUnitOfWork _contentsUnitOfWork;
    private readonly IArticleRepository _articleRepository;

    public ArticleService(
        Func<string, IArticleRepository> articleRepository,
        Func<string, ITopicRepository> topicRepository,
        Func<string, IIdentityRemoteService> identityRemoteService,
        Func<string, IArticleTagService> articleTagService,
        IContentsUnitOfWork contentsUnitOfWork,
        ILogger<ArticleService> logger
    ) : base(articleRepository("v2"), logger)
    {
        _articleTagService = articleTagService("v2");
        _identityRemoteService = identityRemoteService("v2");
        _topicRepository = topicRepository("v2");
        _contentsUnitOfWork = contentsUnitOfWork;
        _articleRepository = articleRepository("v2");
    }

    public async Task<ArticleDto> GetArticleDto(Article article) => await GetArticleDto(article, new ArticleQueryRequest());

    public async Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest)
    {
        var tags = await _articleTagService.GetArticleTags(article.Id);

        UserApiDto? user = null;
        if (articleRequest.IncludeUser)
        {
            user = await _identityRemoteService.GetUserByIdAsync(article.UserId);
        }

        var topic = await _topicRepository.GetByIdAsync(article.TopicId);
        var topicTitle = topic?.Title;

        var articleDto = await buildArticleDto(article, articleRequest, tags, user, topicTitle);

        return articleDto;
    }
    public async Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest)
    {
        if (articles.Count() == 0)
            return new List<ArticleDto>();

        // Get tags for all articles
        Dictionary<int, List<string>> tagDict = await _articleTagService.GetArticleTags([.. articles.Select(a => a.Id).Distinct()]);

        // Get users if requested
        Dictionary<int, UserApiDto?> userDict = new();
        if (articleRequest.IncludeUser)
        {
            var userIds = articles.Select(a => a.UserId).Distinct().ToList();
            var userTasks = userIds.Select(_identityRemoteService.GetUserByIdAsync);
            var users = await Task.WhenAll(userTasks);
            userDict = articles
                .ToDictionary(
                    article => article.Id,
                    article => users.FirstOrDefault(u => u?.UserId == article.UserId)
                );
        }

        // Get topics
        var topicIds = articles.Select(a => a.TopicId).Distinct().ToList();
        var topics = await _topicRepository.GetByIdsAsync(topicIds);
        var topicDict = topics.ToDictionary(t => t.Id, t => t.Title);

        // Build DTOs using the dictionaries
        List<ArticleDto> articleDtos = new();
        foreach (var article in articles)
        {
            var tags = tagDict.GetValueOrDefault(article.Id, new List<string>());
            var user = userDict.GetValueOrDefault(article.Id);
            var topicTitle = topicDict.GetValueOrDefault(article.TopicId);
            articleDtos.Add(await buildArticleDto(article, articleRequest, tags, user, topicTitle));
        }

        return articleDtos;
    }

    private async Task<ArticleDto> buildArticleDto(Article article, ArticleQueryRequest articleRequest, List<string> tags, UserApiDto? user, string? topicTitle)
    {
        var articleDto = article.ToDto();
        if (!articleRequest.IncludeContent)
            articleDto.Content = string.Empty;
        if (articleRequest.IncludeUser)
            articleDto.User = user;
        if (articleRequest.IncludeSubArticles)
            articleDto.SubArticles = await buildSubArticles(articleDto, articleRequest);
        articleDto.Tags = [.. tags.Where(t => !string.IsNullOrEmpty(t))];
        articleDto.TopicTitle = topicTitle ?? "Default Topic";
        return articleDto;
    }
    private async Task<List<ArticleDto>> buildSubArticles(ArticleDto articleDto, ArticleQueryRequest articleRequest)
    {
        List<ArticleDto> articleDtos = new();
        if (articleDto.ArticleType == ArticleTypes.Article)
        {
            List<Article> subArticles = GetQueryable()
                                        .Where(x => x.ParentArticleId == articleDto.ArticleId
                                            && x.ArticleType == ArticleTypes.SubArticle)
                                        .OrderBy(x => x.SortNumber)
                                        .ToList();
            articleDtos = await GetArticleDtos(subArticles, articleRequest);
        }
        return articleDtos;
    }

    public async Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest)
    {
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var article = await _articleRepository.GetByIdAsync(articleEditRequest.ArticleId, true);
            if (article == null)
                throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Article not found");
            // update not null fields
            if (articleEditRequest.IsHidden != null && article.IsHidden != articleEditRequest.IsHidden)
                article.IsHidden = (int)articleEditRequest.IsHidden;
            if (articleEditRequest.Title != null && article.Title != articleEditRequest.Title)
                article.Title = articleEditRequest.Title;
            if (articleEditRequest.Content != null && article.Content != articleEditRequest.Content)
                article.Content = articleEditRequest.Content;
            if (articleEditRequest.Abstraction != null && article.Abstraction != articleEditRequest.Abstraction)
                article.Abstraction = articleEditRequest.Abstraction;
            if (articleEditRequest.TopicId != null && article.TopicId != articleEditRequest.TopicId)
                article.TopicId = (int)articleEditRequest.TopicId;
            if (articleEditRequest.ArticleType != null && article.ArticleType != articleEditRequest.ArticleType)
            {
                if (article.ArticleType == ArticleTypes.Article)
                {
                    await _articleRepository.PromoteSubArticlesToArticles(article.Id);
                }
                article.ArticleType = articleEditRequest.ArticleType;
            }
            if (articleEditRequest.ParentArticleId != null && article.ParentArticleId != articleEditRequest.ParentArticleId)
            {
                article.ParentArticleId = (int)articleEditRequest.ParentArticleId;
            }
            if (articleEditRequest.SortNumber != null && article.SortNumber != articleEditRequest.SortNumber)
                article.SortNumber = (int)articleEditRequest.SortNumber;
            if (articleEditRequest.TagIds != null) await _articleTagService.EditArticleTags(article.Id, articleEditRequest.TagIds);
            //article.UpdateTime = DateTime.UtcNow;
            await _articleRepository.UpdateModifiedAsync(article);
        });
        return true;
    }

    public async Task<bool> DeleteArticleById(int id)
    {
        if (id <= 0)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid article id");
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _articleRepository.PromoteSubArticlesToArticles(id);
            await _articleRepository.DeleteAsync(id);
            await _articleTagService.RemoveArticleTags(id);
            await _contentsUnitOfWork.CommitAsync();
        });
        return true;
    }

    public async Task<Paged<ArticleDto>> GetArticlesByPageRequest(ArticleQueryRequest pageRequest)
    {
        var keyword = pageRequest.Keyword;
        List<int>? topicIds = pageRequest.TopicIds;
        var tagIds = pageRequest.TagIds;

        var query = GetQueryable();
        if (pageRequest.DisplaySubArticles)
        {
            query = query.Where(x => x.ArticleType == ArticleTypes.Article);
        }
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{keyword}%"));
        }
        if (topicIds != null && topicIds.Count > 0)
        {
            query = query.Where(x => topicIds.Contains(x.TopicId));
        }
        if (tagIds != null && tagIds.Count > 0)
        {
            query = _articleRepository.SearchByTagIds(tagIds, query);
        }

        var pagedData = await _articleRepository.GetPagedFromQueryAsync(query, pageRequest);
        var articleList = await GetArticleDtos(pagedData.Data, pageRequest);
        var res = new Paged<ArticleDto>(pagedData.GetPageInfo(), articleList);

        return res;

    }
    public async Task<bool> EditContentBatch(Dictionary<int, string> batchEditConentRequests)
    {
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var articles = await _articleRepository.GetByIdsAsync([.. batchEditConentRequests.Keys], true);
            if (articles == null || articles.Count != batchEditConentRequests.Count)
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid article ids");
            foreach (var article in articles)
            {
                article.Content = batchEditConentRequests[article.Id];
            }
            await _articleRepository.UpdateBatchAsync(articles);
            await _contentsUnitOfWork.CommitAsync();
        });
        return true;
    }

    public async Task<Dictionary<int, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, int userId)
    {
        var result = await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var articles = articleAddRequests.Select(request => request.ToEntity(userId)).ToList();
            var ids = await _articleRepository.CreateBatchAsync(articles);
            // todo: add tags
            return ids.ToDictionary(id => id, id => articles.First(a => a.Id == id).Title);
        });
        return result;
    }
}

