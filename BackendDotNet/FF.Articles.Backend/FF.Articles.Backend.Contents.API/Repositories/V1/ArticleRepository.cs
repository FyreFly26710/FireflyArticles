using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.ElasticSearch;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V1;
public class ArticleRepository(ContentsDbContext _context)
    : BaseRepository<Article, ContentsDbContext>(_context), IArticleRepository
{
    public IQueryable<Article> BuildTagIdsSearchQuery(List<long> tagIds, IQueryable<Article> query)
    {
        return from a in query
               join at in _context.Set<ArticleTag>() on a.Id equals at.ArticleId
               where tagIds.Contains(at.TagId)
               select a;
    }

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
        //await SaveChangesAsync();
    }

    public IQueryable<Article> BuildSearchQueryFromRequest(ArticleQueryRequest request)
    {
        var keyword = request.Keyword;
        List<long>? topicIds = request.TopicIds;
        var tagIds = request.TagIds;
        var displaySubArticles = request.DisplaySubArticles;

        var query = GetQueryable();
        if (!displaySubArticles)
        {
            query = query.Where(x => x.ArticleType == ArticleTypes.Article);
        }
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => EF.Functions.ILike(x.Title, $"%{keyword}%"));
        }
        if (topicIds != null && topicIds.Count > 0)
        {
            query = query.Where(x => topicIds.Contains(x.TopicId));
        }
        if (tagIds != null && tagIds.Count > 0)
        {
            query = BuildTagIdsSearchQuery(tagIds, query);
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