using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;
public class ArticleRepository : BaseRepository<Article, ContentsDbContext>, IArticleRepository
{
    public ArticleRepository(ContentsDbContext _context) : base(_context)
    {
    }



    public IQueryable<Article> SearchByTagIds(List<int> tagIds, IQueryable<Article> query)
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
        await SaveAsync();
    }
}