using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.Services.V2;
public class ArticleService : RedisService<Article>, IArticleService
{
    private readonly IArticleRedisRepository _articleRedisRepository;
    private readonly IIdentityRemoteService _identityRemoteService;
    private readonly IArticleTagRedisRepository _articleTagRedisRepository;
    private readonly ITagRedisRepository _tagRedisRepository;
    private readonly ITopicRedisRepository _topicRedisRepository;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(
        IArticleRedisRepository articleRedisRepository,
        IIdentityRemoteService identityRemoteService,
        IArticleTagRedisRepository articleTagRedisRepository,
        ITagRedisRepository tagRedisRepository,
        ITopicRedisRepository topicRedisRepository,
        ILogger<ArticleService> logger
    ) : base(articleRedisRepository, logger)
    {
        _articleRedisRepository = articleRedisRepository;
        _identityRemoteService = identityRemoteService;
        _logger = logger;
        _articleTagRedisRepository = articleTagRedisRepository;
        _tagRedisRepository = tagRedisRepository;
        _topicRedisRepository = topicRedisRepository;
    }

    public Task<Dictionary<long, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, long userId)
    {
        throw new NotImplementedException();
    }

    public Task<long> CreateByRequest(ArticleAddRequest articleAddRequest, long userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteArticleById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ArticleDto> GetArticleDto(Article article)
    {
        throw new NotImplementedException();
    }

    public Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest)
    {
        throw new NotImplementedException();
    }

    public Task<Paged<ArticleDto>> GetPagedArticlesByRequest(ArticleQueryRequest pageRequest)
    {
        throw new NotImplementedException();
    }

    //public async Task<ArticleDto> GetArticleDto(Article article) => await GetArticleDto(article, new ArticleQueryRequest());

    //public async Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest)
    //{
    //    var articleTags = await _articleTagRedisRepository.GetByArticleId(article.Id);

    //    var tags = await _tagRedisRepository.GetByIdsAsync(articleTags.Select(at => at.TagId).ToList());

    //    UserApiDto? user = null;
    //    if (articleRequest.IncludeUser)
    //    {
    //        user = await _identityRemoteService.GetUserByIdAsync(article.UserId);
    //    }

    //    var topic = await _topicRedisRepository.GetByIdAsync(article.TopicId);
    //    var topicTitle = topic?.Title;

    //    var articleDto = await buildArticleDto(article, articleRequest, tags, user, topicTitle);

    //    return articleDto;
    //}
    //public async Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest)
    //{
    //    if (articles.Count() == 0)
    //        return new List<ArticleDto>();

    //    // Get tags for all articles
    //    Dictionary<long, List<ArticleTag>> articleTagDict = await _articleTagRedisRepository.GetArticleTagGroupsByArticleIds([.. articles.Select(a => a.Id).Distinct()]);
    //    var tagIds = articleTagDict.Values.SelectMany(at => at.Select(a => a.TagId)).Distinct().ToList();
    //    var totalTags = await _tagRedisRepository.GetByIdsAsync(tagIds);
    //    var tagDict = articleTagDict.ToDictionary(
    //        kvp => kvp.Key,
    //        kvp => kvp.Value.Select(at => totalTags.First(t => t.Id == at.TagId)).ToList()
    //        );
    //    // Get users if requested
    //    Dictionary<long, UserApiDto?> userDict = new();
    //    if (articleRequest.IncludeUser)
    //    {
    //        var userIds = articles.Select(a => a.UserId).Distinct().ToList();
    //        // There will only be very few editors in db. No need to add get user by Ids
    //        var userTasks = userIds.Select(_identityRemoteService.GetUserByIdAsync);
    //        var users = await Task.WhenAll(userTasks);
    //        userDict = articles
    //            .ToDictionary(
    //                article => article.Id,
    //                article => users.FirstOrDefault(u => u?.UserId == article.UserId)
    //            );
    //    }

    //    // Get topics
    //    var topicIds = articles.Select(a => a.TopicId).Distinct().ToList();
    //    var topics = await _topicRedisRepository.GetByIdsAsync(topicIds);
    //    var topicDict = topics.ToDictionary(t => t.Id, t => t.Title);

    //    // Build DTOs using the dictionaries
    //    List<ArticleDto> articleDtos = new();
    //    foreach (var article in articles)
    //    {
    //        var tags = tagDict.GetValueOrDefault(article.Id, []);
    //        var user = userDict.GetValueOrDefault(article.Id);
    //        var topicTitle = topicDict.GetValueOrDefault(article.TopicId);
    //        articleDtos.Add(await buildArticleDto(article, articleRequest, tags, user, topicTitle));
    //    }

    //    return articleDtos;
    //}

    //private async Task<ArticleDto> buildArticleDto(Article article, ArticleQueryRequest articleRequest, List<Tag> tags, UserApiDto? user, string? topicTitle)
    //{
    //    var articleDto = article.ToDto();
    //    if (!articleRequest.IncludeContent)
    //        articleDto.Content = string.Empty;
    //    if (articleRequest.IncludeUser)
    //        articleDto.User = user;
    //    if (articleRequest.IncludeSubArticles && articleDto.ArticleType == ArticleTypes.Article)
    //    {
    //        List<Article> subArticles = await _articleRedisRepository.GetChildArticlesAsync(articleDto.ArticleId);
    //        articleDto.SubArticles = await GetArticleDtos(subArticles, articleRequest);
    //    }
    //    articleDto.Tags = tags.Select(t => t.TagName).ToList();
    //    articleDto.TopicTitle = topicTitle ?? "Default Topic";
    //    return articleDto;
    //}

    //public async Task<Paged<ArticleDto>> GetPagedArticlesByRequest(ArticleQueryRequest pageRequest)
    //{
    //    var articles = await _articleRedisRepository.GetArticlesFromRequest(pageRequest);
    //    var pagedData = await _articleRedisRepository.GetPagedAsync(pageRequest, articles);
    //    var articleDtos = await GetArticleDtos(pagedData.Data, pageRequest);
    //    return new Paged<ArticleDto>(pagedData.GetPageInfo(), articleDtos);
    //}

    //public async Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest)
    //{
    //    var article = await _articleRedisRepository.GetByIdAsync(articleEditRequest.ArticleId);
    //    if (article == null)
    //        throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Article not found");
    //    // update not null fields
    //    if (articleEditRequest.IsHidden != null && article.IsHidden != articleEditRequest.IsHidden)
    //        article.IsHidden = (int)articleEditRequest.IsHidden;
    //    if (articleEditRequest.Title != null && article.Title != articleEditRequest.Title)
    //        article.Title = articleEditRequest.Title;
    //    if (articleEditRequest.Content != null && article.Content != articleEditRequest.Content)
    //        article.Content = articleEditRequest.Content;
    //    if (articleEditRequest.Abstract != null && article.Abstract != articleEditRequest.Abstract)
    //        article.Abstract = articleEditRequest.Abstract;
    //    if (articleEditRequest.TopicId != null && article.TopicId != articleEditRequest.TopicId)
    //        article.TopicId = (long)articleEditRequest.TopicId;
    //    if (articleEditRequest.ArticleType != null && article.ArticleType != articleEditRequest.ArticleType)
    //    {
    //        if (article.ArticleType == ArticleTypes.Article)
    //        {
    //            await _articleRedisRepository.PromoteSubArticlesToArticles(article.Id);
    //        }
    //        article.ArticleType = articleEditRequest.ArticleType;
    //    }
    //    if (articleEditRequest.ParentArticleId != null && article.ParentArticleId != articleEditRequest.ParentArticleId)
    //    {
    //        article.ParentArticleId = articleEditRequest.ParentArticleId;
    //    }
    //    if (articleEditRequest.SortNumber != null && article.SortNumber != articleEditRequest.SortNumber)
    //        article.SortNumber = (int)articleEditRequest.SortNumber;
    //    if (articleEditRequest.Tags != null)
    //    {
    //        var tagIds = (await _tagRedisRepository.GetOrCreateByNamesAsync(articleEditRequest.Tags)).Select(t => t.Id).ToList();
    //        await _articleTagRedisRepository.EditArticleTags(article.Id, tagIds);
    //    }
    //    //article.UpdateTime = DateTime.UtcNow;
    //    await _articleRedisRepository.UpdateAsync(article);
    //    return true;
    //}

    //public async Task<bool> DeleteArticleById(long id)
    //{
    //    if (id == 0)
    //        throw new ArgumentException("Invalid article id", nameof(id));
    //    await _articleRedisRepository.PromoteSubArticlesToArticles(id);
    //    await _articleRedisRepository.DeleteAsync(id);
    //    await _articleTagRedisRepository.DeleteByArticleId(id);
    //    return true;
    //}

    ////public async Task<bool> EditContentBatch(Dictionary<long, string> batchEditConentRequests)
    ////{
    ////    var articles = await _articleRedisRepository.GetByIdsAsync(batchEditConentRequests.Keys.ToList());
    ////    if (articles == null || articles.Count != batchEditConentRequests.Count)
    ////        throw new ArgumentException("Invalid article ids");

    ////    await _articleRedisRepository.UpdateContentBatchAsync(batchEditConentRequests);
    ////    return true;
    ////}

    //public async Task<Dictionary<long, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, long userId)
    //{
    //    var articles = articleAddRequests.Select(request => request.ToEntity(userId)).ToList();
    //    articles = await _articleRedisRepository.CreateBatchAsync(articles);
    //    // todo: add tags
    //    var tagNames = articleAddRequests.SelectMany(r => r.Tags)
    //        .Select(t => t.ToLower().Trim())
    //        .Where(t => !string.IsNullOrEmpty(t))
    //        .Distinct()
    //        .ToList();
    //    var tagIds = (await _tagRedisRepository.GetOrCreateByNamesAsync(tagNames)).Select(t => t.Id).ToList();
    //    foreach (var article in articles)
    //    {
    //        await _articleTagRedisRepository.EditArticleTags(article.Id, tagIds);
    //    }
    //    return articles.ToDictionary(a => a.Id, a => a.Title);
    //}

    //public async Task<long> CreateByRequest(ArticleAddRequest articleAddRequest, long userId)
    //{
    //    var entity = articleAddRequest.ToEntity(userId);
    //    var id = await _articleRedisRepository.CreateAsync(entity);
    //    var tagIds = (await _tagRedisRepository.GetOrCreateByNamesAsync(articleAddRequest.Tags)).Select(t => t.Id).ToList();
    //    await _articleTagRedisRepository.EditArticleTags(id, tagIds);
    //    return id;
    //}
}

