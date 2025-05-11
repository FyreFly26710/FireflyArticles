namespace FF.Articles.Backend.Contents.API.ElasticSearch;

public class ElasticsearchSyncService
{
    private readonly IElasticSearchArticleService _esService;
    private readonly ILogger<ElasticsearchSyncService> _logger;
    private readonly IArticleRepository _articleRepository;

    public ElasticsearchSyncService(
        IElasticSearchArticleService esService,
        IArticleRepository articleRepository,
        ILogger<ElasticsearchSyncService> logger)
    {
        _esService = esService;
        _articleRepository = articleRepository;
        _logger = logger;
    }

    public async Task SyncArticleAsync(Article article)
    {
        await _esService.IndexArticleAsync(article);
    }

    public async Task DeleteArticleAsync(long id)
    {
        await _esService.DeleteArticleAsync(id);
    }

    public async Task SyncAllArticlesAsync(IEnumerable<Article> articles)
    {
        foreach (var article in articles)
        {
            await SyncArticleAsync(article);
        }
    }

    public async Task IndexAllExistingArticles(int batchSize = 100)
    {
        try
        {
            _logger.LogInformation("Starting to index all existing articles to Elasticsearch");

            int totalIndexed = 0;
            int offset = 0;
            bool hasMore = true;

            while (hasMore)
            {
                // Get batch of articles
                var articles = await _articleRepository.GetQueryable()
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                if (articles.Count == 0)
                {
                    hasMore = false;
                    break;
                }

                // Index batch
                _logger.LogInformation("Indexing batch of {Count} articles (offset: {Offset})",
                    articles.Count, offset);

                foreach (var article in articles)
                {
                    await _esService.IndexArticleAsync(article);
                    totalIndexed++;
                }

                offset += batchSize;
            }

            _logger.LogInformation("Finished indexing all existing articles. Total indexed: {Count}", totalIndexed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing existing articles to Elasticsearch");
            throw;
        }
    }
}