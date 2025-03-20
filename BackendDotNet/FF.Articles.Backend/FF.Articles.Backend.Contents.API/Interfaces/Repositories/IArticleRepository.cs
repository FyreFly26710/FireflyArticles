using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories;
public interface IArticleRepository : IBaseRepository<Article, ContentsDbContext>
{
    public IQueryable<Article> BuildTagIdsSearchQuery(List<int> tagIds, IQueryable<Article> query);
    public Task PromoteSubArticlesToArticles(int articleId);
    public Task<List<Article>> GetSubArticles(int articleId);
    public IQueryable<Article> BuildSearchQueryFromRequest(ArticleQueryRequest request);
}
