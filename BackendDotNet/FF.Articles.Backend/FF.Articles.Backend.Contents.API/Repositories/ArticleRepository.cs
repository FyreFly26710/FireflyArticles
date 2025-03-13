using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories;
public class ArticleRepository : BaseRepository<Article, ContentsDbContext>, IArticleRepository
{
    public ArticleRepository(ContentsDbContext _context) : base(_context)
    {
    }

    public List<Article> GetSubArticles(int articleId)
    {
        return GetQueryable()
                .Where(x => x.ParentArticleId == articleId
                    && x.ArticleType == ArticleTypes.SubArticle)
                .OrderBy(x => x.SortNumber)
                .ToList();
    }


    public IQueryable<Article> SearchByTagIds(List<int> tagIds, IQueryable<Article> query)
    {
        return from a in query
               join at in _context.Set<ArticleTag>() on a.Id equals at.ArticleId
               where tagIds.Contains(at.TagId)
               select a;
    }
}