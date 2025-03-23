using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.MapperExtensions.Articles;

namespace FF.Articles.Backend.Contents.API.Services.V2;
public class ArticleService : RedisService<Article>, IArticleService
{
    private readonly IArticleRedisRepository _articleRedisRepository;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(IArticleRedisRepository articleRedisRepository, ILogger<ArticleService> logger)
        : base(articleRedisRepository, logger)
    {
        _articleRedisRepository = articleRedisRepository;
        _logger = logger;
    }

    public async Task<ArticleDto> GetArticleDto(Article article) => await GetArticleDto(article, new ArticleQueryRequest());

    public async Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest)
    {
        var articleDto = article.ToDto();

        if (!articleRequest.IncludeContent)
            articleDto.Content = string.Empty;

        if (articleRequest.IncludeSubArticles && articleDto.ArticleType == ArticleTypes.Article)
        {
            var subArticles = await _articleRedisRepository.GetChildArticlesAsync(article.Id);
            articleDto.SubArticles = await GetArticleDtos(subArticles, articleRequest);
        }

        articleDto.TopicTitle = "Default Topic"; // TODO: Implement topic title retrieval
        return articleDto;
    }

    public async Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest)
    {
        if (!articles.Any())
            return new List<ArticleDto>();

        var articleDtos = new List<ArticleDto>();
        foreach (var article in articles)
        {
            articleDtos.Add(await GetArticleDto(article, articleRequest));
        }

        return articleDtos;
    }

    public async Task<Paged<ArticleDto>> GetArticlesByPageRequest(ArticleQueryRequest pageRequest)
    {
        var pagedData = await _articleRedisRepository.GetPagedAsync(pageRequest);
        var articleDtos = await GetArticleDtos(pagedData.Data, pageRequest);
        return new Paged<ArticleDto>(pagedData.GetPageInfo(), articleDtos);
    }

    public async Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest)
    {
        var article = await _articleRedisRepository.GetByIdAsync(articleEditRequest.ArticleId);
        if (article == null)
            throw new KeyNotFoundException($"Article with ID {articleEditRequest.ArticleId} not found");

        // Update not null fields
        if (articleEditRequest.IsHidden != null && article.IsHidden != articleEditRequest.IsHidden)
            article.IsHidden = (int)articleEditRequest.IsHidden;
        if (articleEditRequest.Title != null && article.Title != articleEditRequest.Title)
            article.Title = articleEditRequest.Title;
        if (articleEditRequest.Content != null && article.Content != articleEditRequest.Content)
            article.Content = articleEditRequest.Content;
        if (articleEditRequest.Abstract != null && article.Abstract != articleEditRequest.Abstract)
            article.Abstract = articleEditRequest.Abstract;
        if (articleEditRequest.TopicId != null && article.TopicId != articleEditRequest.TopicId)
            article.TopicId = (int)articleEditRequest.TopicId;
        if (articleEditRequest.ArticleType != null && article.ArticleType != articleEditRequest.ArticleType)
        {
            if (article.ArticleType == ArticleTypes.Article)
            {
                // TODO: Implement promote sub-articles to articles
            }
            article.ArticleType = articleEditRequest.ArticleType;
        }
        if (articleEditRequest.ParentArticleId != null && article.ParentArticleId != articleEditRequest.ParentArticleId)
        {
            article.ParentArticleId = (int)articleEditRequest.ParentArticleId;
        }
        if (articleEditRequest.SortNumber != null && article.SortNumber != articleEditRequest.SortNumber)
            article.SortNumber = (int)articleEditRequest.SortNumber;

        await _articleRedisRepository.UpdateAsync(article);
        return true;
    }

    public async Task<bool> DeleteArticleById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid article id", nameof(id));

        // TODO: Implement promote sub-articles to articles
        return await _articleRedisRepository.DeleteAsync(id);
    }

    public async Task<bool> EditContentBatch(Dictionary<int, string> batchEditConentRequests)
    {
        var articles = await _articleRedisRepository.GetByIdsAsync(batchEditConentRequests.Keys.ToList());
        if (articles == null || articles.Count != batchEditConentRequests.Count)
            throw new ArgumentException("Invalid article ids");

        foreach (var article in articles)
        {
            article.Content = batchEditConentRequests[article.Id];
            article.UpdateTime = DateTime.UtcNow;
        }

        await _articleRedisRepository.UpdateBatchAsync(articles);
        return true;
    }

    public async Task<Dictionary<int, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, int userId)
    {
        var articles = articleAddRequests.Select(request => request.ToEntity(userId)).ToList();
        var count = await _articleRedisRepository.CreateBatchAsync(articles);

        return articles.ToDictionary(
            article => article.Id,
            article => article.Title
        );
    }

    public async Task<int> CreateByRequest(ArticleAddRequest articleAddRequest, int userId)
    {
        var entity = articleAddRequest.ToEntity(userId);
        return await _articleRedisRepository.CreateAsync(entity);
    }
}

