using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
public interface IArticleTagRepository : IBaseRepository<ArticleTag, ContentsDbContext>
{
    Task<bool> EditArticleTags(int articleId, List<int> tagIds);
    Task<bool> DeleteByArticleId(int articleId);
    Task<bool> DeleteByTagId(int tagId);
    Task<List<ArticleTag>> GetByArticleId(int articleId);
    Task<List<ArticleTag>> GetByTagId(int tagId);
    Task<List<ArticleTag>> GetByArticleIds(List<int> articleIds);
    Task<List<ArticleTag>> GetByTagIds(List<int> tagIds);
    Task<Dictionary<int, List<Tag>>> GetTagGroupsByArticleIds(List<int> articleIds);
}


