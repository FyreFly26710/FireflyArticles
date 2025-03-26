using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;

public interface IArticleTagRedisRepository : IRedisRepository<ArticleTag>
{
    Task<List<ArticleTag>> GetByArticleId(long articleId);
    Task<Dictionary<long, List<ArticleTag>>> GetArticleTagGroupsByArticleIds(List<long> articleIds);
    Task<bool> EditArticleTags(long articleId, List<long> tagIds);
    Task<bool> DeleteByArticleId(long articleId);
    Task<bool> DeleteByTagId(long tagId);
    Task<List<ArticleTag>> GetByTagId(long tagId);
    Task<List<ArticleTag>> GetByArticleIds(List<long> articleIds);
    Task<List<ArticleTag>> GetByTagIds(List<long> tagIds);
}
