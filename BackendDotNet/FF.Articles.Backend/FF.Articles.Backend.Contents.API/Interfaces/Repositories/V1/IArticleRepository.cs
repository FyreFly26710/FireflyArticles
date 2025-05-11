using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
public interface IArticleRepository : IBaseRepository<Article, ContentsDbContext>
{
    public Task PromoteSubArticlesToArticles(long articleId);
    public IQueryable<Article> BuildSearchQueryFromRequest(ArticleQueryRequest request);
    public Task SetTopicIdToZero(long topicId);
}
