namespace FF.Articles.Backend.Contents.API.Interfaces.Services;
public interface IArticleService : IBaseService<Article>
{
    Task<ArticleDto> GetArticleDto(Article article);
    Task<ArticleDto> GetArticleDto(Article article, ArticleQueryRequest articleRequest);
    Task<List<ArticleDto>> GetArticleDtos(IEnumerable<Article> articles, ArticleQueryRequest articleRequest);
    Task<bool> EditArticleByRequest(ArticleEditRequest articleEditRequest);
    Task<bool> DeleteArticleById(long id);
    Task<Paged<ArticleDto>> GetPagedArticlesByRequest(ArticleQueryRequest pageRequest);
    //Task<bool> EditContentBatch(Dictionary<long, string> batchEditConentRequests);
    Task<Dictionary<long, string>> CreateBatchAsync(List<ArticleAddRequest> articleAddRequests, long userId);
    Task<long> CreateByRequest(ArticleAddRequest articleAddRequest, long userId);
}
