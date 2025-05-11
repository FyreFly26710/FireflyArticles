namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
public interface IArticleRepository : IBaseRepository<Article, ContentsDbContext>
{
    public Task PromoteSubArticlesToArticles(long articleId);
    public IQueryable<Article> BuildSearchQueryFromRequest(ArticleQueryRequest request);
    public Task SetTopicIdToZero(long topicId);
}
