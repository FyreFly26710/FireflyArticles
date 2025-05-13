namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories;
public interface IArticleTagRepository : IBaseRepository<ArticleTag, ContentsDbContext>
{
    Task<bool> EditArticleTags(long articleId, List<long> tagIds);
    Task<bool> DeleteByArticleId(long articleId);
    Task<bool> DeleteByTagId(long tagId);
    Task<List<ArticleTag>> GetByArticleId(long articleId);
    Task<List<ArticleTag>> GetByTagId(long tagId);
    Task<List<ArticleTag>> GetByArticleIds(List<long> articleIds);
    Task<List<ArticleTag>> GetByTagIds(List<long> tagIds);
    Task<Dictionary<long, List<Tag>>> GetTagGroupsByArticleIds(List<long> articleIds);
}


