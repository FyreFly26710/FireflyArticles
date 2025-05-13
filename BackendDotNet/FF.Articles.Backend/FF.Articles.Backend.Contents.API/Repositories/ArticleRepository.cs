namespace FF.Articles.Backend.Contents.API.Repositories;
public class ArticleRepository(ContentsDbContext _context)
    : BaseRepository<Article, ContentsDbContext>(_context), IArticleRepository
{

    public async Task PromoteSubArticlesToArticles(long articleId)
    {
        var subArticles = GetQueryable().AsTracking()
            .Where(x => x.ParentArticleId == articleId
                && x.ArticleType == ArticleTypes.SubArticle);
        foreach (var subArticle in subArticles)
        {
            subArticle.ParentArticleId = null;
            subArticle.ArticleType = ArticleTypes.Article;
            subArticle.UpdateTime = DateTime.UtcNow;
        }
        await Task.CompletedTask;
        //await SaveChangesAsync();
    }

    public IQueryable<Article> BuildSearchQueryFromRequest(ArticleQueryRequest request)
    {
        var keyword = request.Keyword;
        List<long>? topicIds = request.TopicIds;
        var tagIds = request.TagIds;
        var displaySubArticles = request.DisplaySubArticles;
        var displayTopicArticles = request.DisplayTopicArticles;

        var query = GetQueryable();
        List<string> articleTypes = [ArticleTypes.Article];
        if (displaySubArticles)
        {
            articleTypes.Add(ArticleTypes.SubArticle);
        }
        if (displayTopicArticles)
        {
            articleTypes.Add(ArticleTypes.TopicArticle);
        }
        query = query.Where(x => articleTypes.Contains(x.ArticleType));
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => EF.Functions.ILike(x.Title, $"%{keyword}%"));
            //|| EF.Functions.ILike(x.Abstract, $"%{keyword}%"));
        }
        if (topicIds != null && topicIds.Count > 0)
        {
            query = query.Where(x => topicIds.Contains(x.TopicId));
        }
        if (tagIds != null && tagIds.Count > 0)
        {
            query = from a in query
                    join at in _context.Set<ArticleTag>() on a.Id equals at.ArticleId
                    where tagIds.Contains(at.TagId)
                    select a;
        }
        return query;
    }

    public async Task SetTopicIdToZero(long topicId)
    {
        var articles = await GetQueryable().AsTracking().Where(x => x.TopicId == topicId).ToListAsync();
        foreach (var article in articles)
        {
            article.TopicId = 0;
            article.UpdateTime = DateTime.UtcNow;
        }

    }

}