using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Models.Entities;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2
{
    public interface IArticleRedisRepository : IRedisRepository<Article>
    {
        Task<bool> UpdateAsync(Article newEntity, Article oldEntity);
        Task<List<Article>> GetArticlesByTopicIdAsync(int topicId);
        Task<List<Article>> GetChildArticlesAsync(int parentId);
    }
}
