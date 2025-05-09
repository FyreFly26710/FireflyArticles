﻿using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
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

        public string EntityKey => throw new NotImplementedException();

        public Task<bool> UpdateAsync(Article newEntity, Article oldEntity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> GetArticlesByTopicIdAsync(long topicId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> GetChildArticlesAsync(long parentId)
        {
            throw new NotImplementedException();
        }

        public Task PromoteSubArticlesToArticles(long articleId)
        {
            throw new NotImplementedException();
        }

        public Task SetTopicIdToZero(long topicId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> GetArticlesFromRequest(ArticleQueryRequest request)
        {
            throw new NotImplementedException();
        }

        public string GetFieldKey(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Article?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<long>> ExistIdsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> GetByIdsAsync(List<long> ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<long> CreateAsync(Article entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> CreateBatchAsync(List<Article> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Article entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBatchAsync(List<Article> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBatchAsync(List<long> ids)
        {
            throw new NotImplementedException();
        }

        public Task<Paged<Article>> GetPagedAsync(PageRequest pageRequest, List<Article>? entities = null)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueChangeAsync(Article entity, ChangeType type, ITransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task EnqueueChangesAsync(IEnumerable<Article> entities, ChangeType type, ITransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetQueueLength()
        {
            throw new NotImplementedException();
        }

        public Task<List<DataChange>> PeekChanges(int count)
        {
            throw new NotImplementedException();
        }

        public Task ClearQueue()
        {
            throw new NotImplementedException();
        }
        // private const string ID_COUNTER = "Article:IdCounter";
        //private const string TOPIC_INDEX = "Article:Topic:";
        //private const string PARENT_INDEX = "Article:Parent:";
        // Use Set to store all article ids
        //private const string ARTICLE_IDS_KEY = "Article:Ids";


        //public string EntityKey => "Article";
        //public async Task<bool> UpdateAsync(Article newEntity, Article oldEntity)
        //{
        //    var hash = new List<HashEntry>();
        //    if (newEntity.Title != oldEntity.Title)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.Title));
        //    }
        //    if (newEntity.Content != oldEntity.Content)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.Content));
        //    }
        //    if (newEntity.Abstract != oldEntity.Abstract)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.Abstract));
        //    }
        //    if (newEntity.ArticleType != oldEntity.ArticleType)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.ArticleType));
        //    }
        //    if (newEntity.ParentArticleId != oldEntity.ParentArticleId)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.ParentArticleId));
        //    }
        //    if (newEntity.UserId != oldEntity.UserId)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.UserId));
        //    }
        //    if (newEntity.TopicId != oldEntity.TopicId)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.TopicId));
        //    }
        //    if (newEntity.SortNumber != oldEntity.SortNumber)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.SortNumber));
        //    }
        //    if (newEntity.IsHidden != oldEntity.IsHidden)
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.IsHidden));
        //    }
        //    if (hash.Any())
        //    {
        //        hash.Add(GetHashEntry(newEntity, ArticleProperty.UpdateTime));
        //        await _redis.HashSetAsync(GetFieldKey(newEntity.Id), hash.ToArray());
        //        await UpdateIndexAsync(oldEntity, newEntity);
        //        await EnqueueChangeAsync(newEntity, ChangeType.Update);
        //    }
        //    return true;
        //}

        //public async Task<List<Article>> GetArticlesByTopicIdAsync(long topicId)
        //{
        //    var ids = await _redis.SetMembersAsync($"{RedisIndex.TOPIC_INDEX}{topicId}");
        //    return await GetByIdsAsync(ids.Select(id => (long)id).ToList());
        //}


        //public async Task<List<Article>> GetChildArticlesAsync(long parentId)
        //{
        //    var ids = await _redis.SetMembersAsync($"{RedisIndex.PARENT_INDEX}{parentId}");
        //    return await GetByIdsAsync(ids.Select(id => (long)id).ToList());
        //}

        //public async Task PromoteSubArticlesToArticles(long articleId)
        //{
        //    var ids = await _redis.SetMembersAsync($"{RedisIndex.PARENT_INDEX}{articleId}");
        //    var articles = await GetByIdsAsync(ids.Select(id => (long)id).ToList());
        //    foreach (var article in articles)
        //    {
        //        var oldEntity = article.Clone();
        //        oldEntity.Id = article.Id;
        //        article.ParentArticleId = null;
        //        article.ArticleType = ArticleTypes.Article;
        //        await UpdateAsync(article, oldEntity);
        //    }
        //}

        ////public async Task UpdateContentBatchAsync(Dictionary<long, string> batchEditConentRequests)
        ////{
        ////    var articles = await GetByIdsAsync(batchEditConentRequests.Keys.ToList());
        ////    foreach (var request in batchEditConentRequests)
        ////    {
        ////        var hash = new List<HashEntry>();
        ////        hash.Add(new HashEntry(ArticleProperty.Content, request.Value));
        ////        hash.Add(new HashEntry(ArticleProperty.UpdateTime, DateTime.UtcNow.ToString("O")));
        ////        await _redis.HashSetAsync(GetFieldKey(request.Key), hash.ToArray());
        ////        //await UpdateIndexAsync(oldEntity, newEntity);
        ////        await EnqueueChangeAsync(articles.First(a => a.Id == request.Key), ChangeType.Update);
        ////    }
        ////}
        //public async Task SetTopicIdToZero(long topicId)
        //{
        //    var articles = (await GetAllAsync()).Where(a => a.TopicId == topicId).ToList();
        //    foreach (var article in articles)
        //    {
        //        var oldEntity = article.Clone();
        //        var hash = new List<HashEntry>();
        //        article.TopicId = 0;
        //        hash.Add(new HashEntry(ArticleProperty.TopicId, 0));
        //        hash.Add(new HashEntry(ArticleProperty.UpdateTime, DateTime.UtcNow.ToString("O")));
        //        await _redis.HashSetAsync(GetFieldKey(article.Id), hash.ToArray());
        //        await UpdateIndexAsync(oldEntity, article);
        //        await EnqueueChangeAsync(article, ChangeType.Update);
        //    }
        //}
        //public async Task<List<Article>> GetArticlesFromRequest(ArticleQueryRequest request)
        //{
        //    var keyword = request.Keyword;
        //    List<long>? topicIds = request.TopicIds;
        //    var tagIds = request.TagIds;
        //    var displaySubArticles = request.DisplaySubArticles;
        //    var articles = new List<Article>();
        //    if (topicIds != null && topicIds.Count > 0)
        //    {
        //        var ids = await _redis.SetMembersAsync(string.Join(",", topicIds.Select(id => $"{RedisIndex.TOPIC_INDEX}{id}")));
        //        articles = await GetByIdsAsync(ids.Select(id => (long)id).ToList());
        //    }
        //    else if (topicIds == null || topicIds.Count == 0)
        //    {
        //        articles = await GetAllAsync();
        //    }
        //    // TODO: filter by tagIds
        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        articles = articles.Where(x => x.Title.Contains(keyword.Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();
        //    }

        //    if (!displaySubArticles)
        //    {
        //        articles = articles.Where(x => x.ArticleType == ArticleTypes.Article).ToList();
        //    }


        //    return articles;
        //}
    }
}