using Nest;

namespace FF.Articles.Backend.Contents.API.ElasticSearch;
public interface IElasticSearchArticleService
{
    Task IndexArticleAsync(Article article);
    Task<Paged<Article>> SearchArticlesAsync(string keyword, int size = 50, int from = 0,
        List<long>? topicIds = null, List<long>? articleIds = null);
    Task DeleteArticleAsync(long id);
    Task ReCreateIndexAsync();
    Task<int> GetArticleIndexCount();
}
public class ESArticleService(IElasticClient _elasticClient, ILogger<ESArticleService> _logger)
    : IElasticSearchArticleService
{
    public async Task IndexArticleAsync(Article article)
    {
        var response = await _elasticClient.IndexDocumentAsync(article);
        if (!response.IsValid)
        {
            _logger.LogError("Failed to index article {ArticleId}: {ErrorMessage}", article.Id, response.DebugInformation);
        }
    }

    public async Task<Paged<Article>> SearchArticlesAsync(string keyword, int size = 50, int from = 0,
        List<long>? topicIds = null, List<long>? articleIds = null)
    {
        // Ensure we have a search term
        if (string.IsNullOrWhiteSpace(keyword))
        {
            _logger.LogWarning("Empty search keyword provided");
            return new Paged<Article>(from, size, 0, new List<Article>());
        }

        try
        {
            var searchResponse = await _elasticClient.SearchAsync<Article>(s => s
                .From(from)
                .Size(size)
                .Query(q => q
                    .Bool(b =>
                    {
                        var boolQuery = b.Should(
                            // Match query for partial matching on multiple terms
                            s => s.Match(m => m
                                .Field(f => f.Title)
                                .Query(keyword)
                                .Boost(3) // Title matches are most important
                                .Fuzziness(Fuzziness.Auto)
                            ),
                            s => s.Match(m => m
                                .Field(f => f.Abstract)
                                .Query(keyword)
                                .Boost(2) // Abstract matches are second most important
                                .Fuzziness(Fuzziness.Auto)
                            ),
                            // Match phrase for exact phrase matching
                            s => s.MatchPhrase(mp => mp
                                .Field(f => f.Title)
                                .Query(keyword)
                                .Boost(5) // Exact phrase matches are highest priority
                            ),
                            s => s.MatchPhrase(mp => mp
                                .Field(f => f.Abstract)
                                .Query(keyword)
                                .Boost(4)
                            )
                        );

                        // Require at least one of the should clauses to match
                        boolQuery = boolQuery.MinimumShouldMatch(1);

                        // Add filters - these don't affect relevance score but narrow results
                        // Filter by topic IDs if specified
                        if (topicIds != null && topicIds.Count > 0)
                            boolQuery = boolQuery.Filter(f => f.Terms(t => t.Field(field => field.TopicId).Terms(topicIds)));

                        // Filter by specific article IDs (useful for tag filtering)
                        if (articleIds != null && articleIds.Count > 0)
                            boolQuery = boolQuery.Filter(f => f.Terms(t => t.Field(field => field.Id).Terms(articleIds)));

                        return boolQuery;
                    })
                )
                .Sort(sort => sort.Descending(SortSpecialField.Score)) // Sort by relevance
            );

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Elasticsearch query failed: {ErrorMessage}", searchResponse.DebugInformation);
                return new Paged<Article>(from, size, 0, new List<Article>());
            }

            var data = searchResponse.Documents.ToList();
            return new Paged<Article>(from, size, (int)searchResponse.Total, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing search for '{Keyword}'", keyword);
            return new Paged<Article>(from, size, 0, new List<Article>());
        }
    }

    public async Task DeleteArticleAsync(long id)
    {
        await _elasticClient.DeleteAsync<Article>(id);
    }

    public async Task ReCreateIndexAsync()
    {
        var indexExistsResponse = await _elasticClient.Indices.ExistsAsync("articles");
        if (indexExistsResponse.Exists)
        {
            await _elasticClient.Indices.DeleteAsync("articles");
        }
        var createResponse = await _elasticClient.Indices.CreateAsync("articles", c => c
                .Map<Article>(m => m
                    .Properties(ps => ps
                        // Searchable text fields with analyzers
                        .Text(t => t.Name(n => n.Title).Analyzer("standard"))
                        .Text(t => t.Name(n => n.Abstract).Analyzer("standard"))

                        // Store content but don't analyze it (saves resources)
                        .Text(t => t.Name(n => n.Content).Index(false))

                        // Exact-match filterable fields (keywords)
                        .Keyword(k => k.Name(n => n.ArticleType))
                        .Keyword(k => k.Name(n => n.IsDelete))

                        // Numeric fields (for filtering and sorting)
                        .Number(n => n.Name(p => p.Id).Type(NumberType.Long))
                        .Number(n => n.Name(p => p.TopicId).Type(NumberType.Long))
                        .Number(n => n.Name(p => p.ParentArticleId).Type(NumberType.Long))
                        .Number(n => n.Name(p => p.UserId).Type(NumberType.Long))
                        .Number(n => n.Name(p => p.SortNumber).Type(NumberType.Integer))
                        .Number(n => n.Name(p => p.IsHidden).Type(NumberType.Integer))

                        // Date fields (for sorting by date)
                        .Date(d => d.Name(p => p.CreateTime))
                        .Date(d => d.Name(p => p.UpdateTime))
                    )
                )
            );

        if (!createResponse.IsValid)
        {
            _logger.LogError("Failed to create index: {Error}", createResponse.DebugInformation);
        }
    }

    public async Task<int> GetArticleIndexCount()
    {
        try
        {
            // Check if we can connect, if not return -1
            var pingResponse = await _elasticClient.PingAsync();
            if (!pingResponse.IsValid)
                return -1;

            // Check if index exists, if not return 0
            var indexExistsResponse = await _elasticClient.Indices.ExistsAsync("articles");
            if (!indexExistsResponse.Exists)
                return 0;

            // Check if index has any documents
            var countResponse = await _elasticClient.CountAsync<Article>(c => c.Index("articles"));
            if (!countResponse.IsValid || countResponse.Count == 0)
                return 0;

            // All checks passed
            return (int)countResponse.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch readiness check failed");
            return -1;
        }
    }
}
