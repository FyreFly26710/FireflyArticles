using AutoMapper;
using Azure.Core;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.MapperExtensions.Articles;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Services.V1;
public class ArticleService : BaseService<Article, ContentsDbContext>, IArticleService
{
    private readonly IIdentityRemoteService _identityRemoteService;
    private readonly ITagRepository _tagRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IContentsUnitOfWork _contentsUnitOfWork;
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleTagRepository _articleTagRepository;

    public ArticleService(
        IArticleRepository articleRepository,
        ITagRepository tagRepository,
        ITopicRepository topicRepository,
        IIdentityRemoteService identityRemoteService,
        IArticleTagRepository articleTagRepository,
        IContentsUnitOfWork contentsUnitOfWork,
        ILogger<ArticleService> logger
    ) : base(articleRepository, logger)
    {
        _tagRepository = tagRepository;
        _identityRemoteService = identityRemoteService;
        _topicRepository = topicRepository;
        _contentsUnitOfWork = contentsUnitOfWork;
        _articleRepository = articleRepository;
        _articleTagRepository = articleTagRepository;
    }

    public async Task<ArticleDto> GetArticleDto(Article article) => await GetArticleDto(article, new ArticleQueryRequest());

    public async Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest)
    {
        var articleTags = await _articleTagRepository.GetByArticleId(article.Id);

        var tags = await _tagRepository.GetByIdsAsync(articleTags.Select(at => at.TagId).ToList());

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
        Dictionary<long, List<Tag>> tagDict = await _articleTagRepository.GetTagGroupsByArticleIds([.. articles.Select(a => a.Id).Distinct()]);

        // Get users if requested
        Dictionary<long, UserApiDto?> userDict = new();
        if (articleRequest.IncludeUser)
        {
            var userIds = articles.Select(a => a.UserId).Distinct().ToList();
            // There will only be very few editors in db. No need to add get user by Ids
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
            var tags = tagDict.GetValueOrDefault(article.Id, []);
            var user = userDict.GetValueOrDefault(article.Id);
            var topicTitle = topicDict.GetValueOrDefault(article.TopicId);
            articleDtos.Add(await buildArticleDto(article, articleRequest, tags, user, topicTitle));
        }

        return articleDtos;
    }

    private async Task<ArticleDto> buildArticleDto(Article article, ArticleQueryRequest articleRequest, List<Tag> tags, UserApiDto? user, string? topicTitle)
    {
        var articleDto = article.ToDto();
        if (!articleRequest.IncludeContent)
            articleDto.Content = string.Empty;
        if (articleRequest.IncludeUser)
            articleDto.User = user;
        if (articleRequest.IncludeSubArticles && articleDto.ArticleType == ArticleTypes.Article)
        {
            List<Article> subArticles = await _articleRepository.GetQueryable()
                .Where(x => x.ParentArticleId == articleDto.ArticleId && x.ArticleType == ArticleTypes.SubArticle)
                .ToListAsync();
            articleDto.SubArticles = await GetArticleDtos(subArticles, articleRequest);
        }
        articleDto.Tags = tags.Select(t => t.TagName).ToList();
        articleDto.TopicTitle = topicTitle ?? "Default Topic";
        return articleDto;
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
            if (articleEditRequest.Abstract != null && article.Abstract != articleEditRequest.Abstract)
                article.Abstract = articleEditRequest.Abstract;
            if (articleEditRequest.TopicId != null && article.TopicId != articleEditRequest.TopicId)
                article.TopicId = (long)articleEditRequest.TopicId;
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
                article.ParentArticleId = articleEditRequest.ParentArticleId;
            }
            if (articleEditRequest.SortNumber != null && article.SortNumber != articleEditRequest.SortNumber)
                article.SortNumber = (int)articleEditRequest.SortNumber;
            if (articleEditRequest.TagNames != null)
            {
                var tagIds = (await _tagRepository.GetOrCreateByNamesAsync(articleEditRequest.TagNames)).Select(t => t.Id).ToList();
                await _articleTagRepository.EditArticleTags(article.Id, tagIds);
            }
            //article.UpdateTime = DateTime.UtcNow;
            await _articleRepository.UpdateAsync(article);
        });
        return true;
    }

    public async Task<bool> DeleteArticleById(long id)
    {
        if (id == 0)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid article id");
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _articleRepository.PromoteSubArticlesToArticles(id);
            await _articleRepository.DeleteAsync(id);
            await _articleTagRepository.DeleteByArticleId(id);
        });
        return true;
    }

    public async Task<Paged<ArticleDto>> GetPagedArticlesByRequest(ArticleQueryRequest pageRequest)
    {
        var query = _articleRepository.BuildSearchQueryFromRequest(pageRequest);
        var pagedData = await _articleRepository.GetPagedFromQueryAsync(query, pageRequest);
        var articleList = await GetArticleDtos(pagedData.Data, pageRequest);
        var res = new Paged<ArticleDto>(pagedData.GetPageInfo(), articleList);

        return res;

    }
    public async Task<bool> EditContentBatch(Dictionary<long, string> batchEditConentRequests)
    {
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var articles = await _articleRepository.GetByIdsAsync([.. batchEditConentRequests.Keys], true);
            if (articles == null || articles.Count != batchEditConentRequests.Count)
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid article ids");
            foreach (var article in articles)
            {
                article.Content = batchEditConentRequests[article.Id];
                article.UpdateTime = DateTime.UtcNow;
            }
            //await _articleRepository.UpdateBatchAsync(articles);
        });
        return true;
    }

    /// <summary>
    /// Create articles and return a dictionary of article id and title
    /// </summary>
    public async Task<Dictionary<long, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, long userId)
    {
        var result = await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var articles = articleAddRequests.Select(request => request.ToEntity(userId)).ToList();
            articles = await _articleRepository.CreateBatchAsync(articles);
            // todo: add tags
            var tagNames = articleAddRequests.SelectMany(r => r.Tags)
                .Select(t => t.ToLower().Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToList();
            var tagIds = (await _tagRepository.GetOrCreateByNamesAsync(tagNames)).Select(t => t.Id).ToList();
            foreach (var article in articles)
            {
                await _articleTagRepository.EditArticleTags(article.Id, tagIds);
            }
            return articles.ToDictionary(a => a.Id, a => a.Title);
        });
        return result;
    }
    public async Task<long> CreateByRequest(ArticleAddRequest articleAddRequest, long userId)
    {
        var entity = articleAddRequest.ToEntity(userId);
        var result = await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var id = await _articleRepository.CreateAsync(entity);
            var tagIds = (await _tagRepository.GetOrCreateByNamesAsync(articleAddRequest.Tags)).Select(t => t.Id).ToList();
            await _articleTagRepository.EditArticleTags(id, tagIds);
            return id;
        });
        return result;
    }
}

