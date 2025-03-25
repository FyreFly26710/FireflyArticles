using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace FF.Articles.Backend.Contents.API.Repositories.V2
{
    /// <summary>
    /// Use Different data structure to store article data
    /// Key: Article:{id}
    /// Field: Title, Content, Abstract, ArticleType, ParentArticleId, UserId, TopicId, SortNumber, IsHidden, CreateTime, UpdateTime
    /// Index: Topic:{topicId}, User:{userId}, Parent:{parentId}
    /// Todo: store content separately, use different data structure and store in md file
    /// </summary>
    public partial class ArticleRedisRepository : IArticleRedisRepository
    {
        public ArticleRedisRepository(IConnectionMultiplexer redis, ILogger<ArticleRedisRepository> logger)
        {
            _redis = redis.GetDatabase();
            _logger = logger;
        }
        private const string KEY_PREFIX = "Article:";
        private const string ID_COUNTER = "Article:IdCounter";
        private const string TOPIC_INDEX = "Article:Topic:";
        private const string PARENT_INDEX = "Article:Parent:";
        // Use Set to store all article ids
        private const string ARTICLE_IDS_KEY = "Article:Ids";


        public string EntityKey => "Article";
        public async Task<bool> UpdateAsync(Article newEntity, Article oldEntity)
        {
            var hash = new List<HashEntry>();
            if (newEntity.Title != oldEntity.Title)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.Title));
            }
            if (newEntity.Content != oldEntity.Content)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.Content));
            }
            if (newEntity.Abstract != oldEntity.Abstract)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.Abstract));
            }
            if (newEntity.ArticleType != oldEntity.ArticleType)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.ArticleType));
            }
            if (newEntity.ParentArticleId != oldEntity.ParentArticleId)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.ParentArticleId));
            }
            if (newEntity.UserId != oldEntity.UserId)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.UserId));
            }
            if (newEntity.TopicId != oldEntity.TopicId)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.TopicId));
            }
            if (newEntity.SortNumber != oldEntity.SortNumber)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.SortNumber));
            }
            if (newEntity.IsHidden != oldEntity.IsHidden)
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.IsHidden));
            }
            if (hash.Any())
            {
                hash.Add(GetHashEntry(newEntity, ArticleProperty.UpdateTime));
                await _redis.HashSetAsync(GetFieldKey(newEntity.Id), hash.ToArray());
                await UpdateIndexAsync(oldEntity, newEntity);
                await EnqueueChangeAsync(newEntity, ChangeType.Update);
            }
            return true;
        }

        public async Task<List<Article>> GetArticlesByTopicIdAsync(int topicId)
        {
            var ids = await _redis.SetMembersAsync($"{TOPIC_INDEX}{topicId}");
            return await GetByIdsAsync(ids.Select(id => (int)id).ToList());
        }


        public async Task<List<Article>> GetChildArticlesAsync(int parentId)
        {
            var ids = await _redis.SetMembersAsync($"{PARENT_INDEX}{parentId}");
            return await GetByIdsAsync(ids.Select(id => (int)id).ToList());
        }

        public async Task PromoteSubArticlesToArticles(int articleId)
        {
            var ids = await _redis.SetMembersAsync($"{PARENT_INDEX}{articleId}");
            var articles = await GetByIdsAsync(ids.Select(id => (int)id).ToList());
            foreach (var article in articles)
            {
                var oldEntity = article.Clone();
                oldEntity.Id = article.Id;
                article.ParentArticleId = null;
                article.ArticleType = ArticleTypes.Article;
                await UpdateAsync(article, oldEntity);
            }
        }

        public async Task UpdateContentBatchAsync(Dictionary<int, string> batchEditConentRequests)
        {
            var articles = await GetByIdsAsync(batchEditConentRequests.Keys.ToList());
            foreach (var request in batchEditConentRequests)
            {
                var hash = new List<HashEntry>();
                hash.Add(new HashEntry(ArticleProperty.Content, request.Value));
                hash.Add(new HashEntry(ArticleProperty.UpdateTime, DateTime.UtcNow.ToString("O")));
                await _redis.HashSetAsync(GetFieldKey(request.Key), hash.ToArray());
                //await UpdateIndexAsync(oldEntity, newEntity);
                await EnqueueChangeAsync(articles.First(a => a.Id == request.Key), ChangeType.Update);
            }
        }
        public async Task SetTopicIdToZero(int topicId)
        {
            var articles = (await GetAllAsync()).Where(a => a.TopicId == topicId).ToList();
            foreach (var article in articles)
            {
                var oldEntity = article.Clone();
                var hash = new List<HashEntry>();
                article.TopicId = 0;
                hash.Add(new HashEntry(ArticleProperty.TopicId, 0));
                hash.Add(new HashEntry(ArticleProperty.UpdateTime, DateTime.UtcNow.ToString("O")));
                await _redis.HashSetAsync(GetFieldKey(article.Id), hash.ToArray());
                await UpdateIndexAsync(oldEntity, article);
                await EnqueueChangeAsync(article, ChangeType.Update);
            }
        }

    }
}