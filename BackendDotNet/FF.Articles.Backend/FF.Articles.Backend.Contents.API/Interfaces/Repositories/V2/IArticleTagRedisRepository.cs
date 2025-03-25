using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;

public interface IArticleTagRedisRepository : IRedisRepository<ArticleTag>
{
    Task<List<ArticleTag>> GetByArticleId(int articleId);
    Task<Dictionary<int, List<ArticleTag>>> GetArticleTagGroupsByArticleIds(List<int> articleIds);
    Task<bool> EditArticleTags(int articleId, List<int> tagIds);
    Task<bool> DeleteByArticleId(int articleId);
    Task<bool> DeleteByTagId(int tagId);
    Task<List<ArticleTag>> GetByTagId(int tagId);
    Task<List<ArticleTag>> GetByArticleIds(List<int> articleIds);
    Task<List<ArticleTag>> GetByTagIds(List<int> tagIds);
}
