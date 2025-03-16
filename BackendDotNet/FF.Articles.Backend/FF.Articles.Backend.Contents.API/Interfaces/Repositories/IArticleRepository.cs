using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories;
public interface IArticleRepository : IBaseRepository<Article, ContentsDbContext>
{
    public IQueryable<Article> SearchByTagIds(List<int> tagIds, IQueryable<Article> query);
    public Task PromoteSubArticlesToArticles(int articleId);
}
