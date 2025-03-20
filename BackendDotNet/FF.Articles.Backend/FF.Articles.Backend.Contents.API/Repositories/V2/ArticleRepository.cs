using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;
public class ArticleRepository : BaseRepository<Article, ContentsDbContext>, IArticleRepository
{
    public ArticleRepository(ContentsDbContext _context) : base(_context)
    {
    }



    public IQueryable<Article> BuildTagIdsSearchQuery(List<int> tagIds, IQueryable<Article> query)
    {
        return from a in query
               join at in _context.Set<ArticleTag>() on a.Id equals at.ArticleId
               where tagIds.Contains(at.TagId)
               select a;
    }

    public async Task PromoteSubArticlesToArticles(int articleId)
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
        List<int>? topicIds = request.TopicIds;
        var tagIds = request.TagIds;
        var displaySubArticles = request.DisplaySubArticles;

        var query = GetQueryable();
        if (displaySubArticles)
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
            query = BuildTagIdsSearchQuery(tagIds, query);
        }
        return query;
    }
    public async Task SetTopicIdToZero(int topicId)
    {
        var articles = await GetQueryable().AsTracking().Where(x => x.TopicId == topicId).ToListAsync();
        foreach (var article in articles)
        {
            article.TopicId = 0;
            article.UpdateTime = DateTime.UtcNow;
        }
        
    }

}