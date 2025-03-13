using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Repositories.Interfaces;
public interface IArticleRepository : IBaseRepository<Article, ContentsDbContext>
{
    public List<Article> GetSubArticles(int articleId);    
    public IQueryable<Article> SearchByTagIds(List<int> tagIds, IQueryable<Article> query);
}
