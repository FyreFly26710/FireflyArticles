using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2
{
    public interface IArticleRedisRepository : IRedisRepository<Article>
    {
        Task<bool> UpdateAsync(Article newEntity, Article oldEntity);
        Task<List<Article>> GetArticlesByTopicIdAsync(long topicId);
        Task<List<Article>> GetChildArticlesAsync(long parentId);
        Task PromoteSubArticlesToArticles(long articleId);
        //Task UpdateContentBatchAsync(Dictionary<long, string> batchEditConentRequests);
        Task SetTopicIdToZero(long topicId);
        Task<List<Article>> GetArticlesFromRequest(ArticleQueryRequest request);
    }
}
