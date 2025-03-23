using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
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
        private readonly IDatabase _redis;
        private readonly ILogger<ArticleRedisRepository> _logger;
        private const string KEY_PREFIX = "Article:";
        private const string ID_COUNTER = "Article:IdCounter";
        private const string TOPIC_INDEX = "Article:Topic:";
        private const string PARENT_INDEX = "Article:Parent:";
        // Use Set to store all article ids
        private const string ARTICLE_IDS_KEY = "Article:Ids";


        public ArticleRedisRepository(IConnectionMultiplexer redis, ILogger<ArticleRedisRepository> logger)
        {
            _redis = redis.GetDatabase();
            _logger = logger;
        }

        public string EntityKey => "Article";

        public async Task<int> GetNextIdAsync()
        {
            return (int)await _redis.StringIncrementAsync(ID_COUNTER);
        }

        public async Task<int> GetNextIdAsync(int count)
        {
            long startId = await _redis.StringIncrementAsync(ID_COUNTER, count);
            return (int)(startId - count + 1);
        }

        public string GetFieldKey(int id) => $"{KEY_PREFIX}{id}";

        public async Task<bool> ExistsAsync(int id)
        {
            return await _redis.KeyExistsAsync(GetFieldKey(id));
        }

        public async Task<List<int>> ExistIdsAsync()
        {
            var members = await _redis.SetMembersAsync(ARTICLE_IDS_KEY);
            return members.Select(m => (int)m).ToList();
        }

        public async Task<Article?> GetByIdAsync(int id)
        {
            var key = GetFieldKey(id);
            var hash = await _redis.HashGetAllAsync(key);

            if (!hash.Any()) return null;

            return GetArticleFromHashEntry(hash, id);
        }


        public async Task<List<Article>> GetByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Article>();

            // Create a pipeline for batch operations
            var batch = _redis.CreateBatch();

            // Queue batch hash gets
            var tasks = ids.Select(id => batch.HashGetAllAsync(GetFieldKey(id))).ToList();

            batch.Execute(); // executes all queued batch operations
            var results = await Task.WhenAll(tasks);

            // Process each HashEntry[] to dictionary and map to Article
            var articles = new List<Article>();

            for (int i = 0; i < results.Length; i++)
            {
                var hash = results[i];

                if (!hash.Any()) continue;

                var article = GetArticleFromHashEntry(hash, ids[i]);
                articles.Add(article);
            }

            return articles;
        }


        public async Task<List<Article>> GetAllAsync()
        {
            var ids = await ExistIdsAsync();
            return await GetByIdsAsync(ids);
        }

        public async Task<int> CreateAsync(Article entity)
        {
            entity.Id = await GetNextIdAsync();
            entity.CreateTime ??= DateTime.UtcNow;
            entity.UpdateTime ??= DateTime.UtcNow;

            var key = GetFieldKey(entity.Id);
            var hash = GetHashEntries(entity);

            await _redis.HashSetAsync(key, hash);

            // Update indexes
            await UpdateIndexAsync(null, entity);

            await EnqueueChangeAsync(entity, ChangeType.Create);
            return entity.Id;
        }

        public async Task<int> CreateBatchAsync(List<Article> entities)
        {
            var baseId = await GetNextIdAsync(entities.Count);
            var tasks = new List<Task>();

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                entity.Id = baseId + i;
                entity.CreateTime ??= DateTime.UtcNow;
                entity.UpdateTime ??= DateTime.UtcNow;

                var key = GetFieldKey(entity.Id);
                var hash = GetHashEntries(entity);

                tasks.Add(_redis.HashSetAsync(key, hash));
                tasks.Add(_redis.SetAddAsync($"{TOPIC_INDEX}{entity.TopicId}", entity.Id));
                if (entity.ParentArticleId.HasValue)
                {
                    tasks.Add(_redis.SetAddAsync($"{PARENT_INDEX}{entity.ParentArticleId}", entity.Id));
                }
            }

            await Task.WhenAll(tasks);
            await EnqueueChangesAsync(entities, ChangeType.Create);
            return entities.Count;
        }

        public async Task<bool> UpdateAsync(Article newEntity)
        {
            if (!await ExistsAsync(newEntity.Id))
                throw new KeyNotFoundException($"Article with ID {newEntity.Id} not found.");

            newEntity.UpdateTime ??= DateTime.UtcNow;
            var key = GetFieldKey(newEntity.Id);
            var hash = GetHashEntries(newEntity);

            var oldEntity = await GetByIdAsync(newEntity.Id);
            await _redis.HashSetAsync(key, hash);
            await UpdateIndexAsync(oldEntity, newEntity);
            await EnqueueChangeAsync(newEntity, ChangeType.Update);
            return true;
        }
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
        public async Task UpdateBatchAsync(List<Article> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var article = await GetByIdAsync(id);
            if (article == null) return false;

            var key = GetFieldKey(id);
            await _redis.KeyDeleteAsync(key);

            // Remove from indexes
            await UpdateIndexAsync(article, null);

            await EnqueueChangeAsync(new Article { Id = id }, ChangeType.Delete);
            return true;
        }
        private async Task UpdateIndexAsync(Article? oldEntity, Article? newEntity)
        {
            // Early return if both are null
            if (oldEntity == null && newEntity == null)
                return;

            var batch = _redis.CreateBatch();
            var tasks = new List<Task>();

            // Delete case: only oldEntity exists
            if (oldEntity != null && newEntity == null)
            {
                tasks.Add(batch.SetRemoveAsync($"{TOPIC_INDEX}{oldEntity.TopicId}", oldEntity.Id));
                if (oldEntity.ParentArticleId.HasValue)
                {
                    tasks.Add(batch.SetRemoveAsync($"{PARENT_INDEX}{oldEntity.ParentArticleId}", oldEntity.Id));
                }
                tasks.Add(batch.SetRemoveAsync(ARTICLE_IDS_KEY, oldEntity.Id));
            }
            // Create case: only newEntity exists
            else if (newEntity != null && oldEntity == null)
            {
                tasks.Add(batch.SetAddAsync($"{TOPIC_INDEX}{newEntity.TopicId}", newEntity.Id));
                if (newEntity.ParentArticleId.HasValue)
                {
                    tasks.Add(batch.SetAddAsync($"{PARENT_INDEX}{newEntity.ParentArticleId}", newEntity.Id));
                }
                tasks.Add(batch.SetAddAsync(ARTICLE_IDS_KEY, newEntity.Id));
            }
            // Update case: both entities exist
            else if (oldEntity != null && newEntity != null)
            {
                if (oldEntity.TopicId != newEntity.TopicId)
                {
                    tasks.Add(batch.SetRemoveAsync($"{TOPIC_INDEX}{oldEntity.TopicId}", oldEntity.Id));
                    tasks.Add(batch.SetAddAsync($"{TOPIC_INDEX}{newEntity.TopicId}", newEntity.Id));
                }
                if (oldEntity.ParentArticleId != newEntity.ParentArticleId)
                {
                    if (oldEntity.ParentArticleId.HasValue)
                    {
                        tasks.Add(batch.SetRemoveAsync($"{PARENT_INDEX}{oldEntity.ParentArticleId}", oldEntity.Id));
                    }
                    if (newEntity.ParentArticleId.HasValue)
                    {
                        tasks.Add(batch.SetAddAsync($"{PARENT_INDEX}{newEntity.ParentArticleId}", newEntity.Id));
                    }
                }
            }

            try
            {
                batch.Execute();
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update indexes for article {ArticleId}",
                    newEntity?.Id ?? oldEntity?.Id);
                throw;
            }
        }

        public async Task<bool> DeleteBatchAsync(List<int> ids)
        {
            var tasks = new List<Task>();
            foreach (var id in ids)
            {
                tasks.Add(DeleteAsync(id));
            }
            await Task.WhenAll(tasks);
            return true;
        }

        public async Task<Paged<Article>> GetPagedAsync(PageRequest pageRequest, List<Article>? entities = null)
        {
            var all = entities ?? await GetAllAsync();
            var total = all.Count;
            var items = all
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .ToList();

            return new Paged<Article>(pageRequest.PageNumber, pageRequest.PageSize, total, items);
        }

        public async Task EnqueueChangeAsync(Article entity, ChangeType type, ITransaction? transaction = null)
        {
            await EnqueueChangesAsync(new[] { entity }, type, transaction);
        }

        public async Task EnqueueChangesAsync(IEnumerable<Article> entities, ChangeType type, ITransaction? transaction = null)
        {
            var changes = entities.Select(entity => new DataChange
            {
                FullName = typeof(Article).FullName!,
                EntityType = EntityKey,
                Id = entity.Id,
                ChangeType = type,
                PayloadJson = type == ChangeType.Delete ? null : JsonSerializer.Serialize(entity)
            });

            var jsonValues = changes.Select(change => (RedisValue)JsonSerializer.Serialize(change)).ToArray();

            if (transaction != null)
            {
                _ = transaction.ListRightPushAsync("DataChangeQueue", jsonValues);
            }
            else
            {
                await _redis.ListRightPushAsync("DataChangeQueue", jsonValues);
            }
        }

        public async Task<int> GetQueueLength() => (int)await _redis.ListLengthAsync("DataChangeQueue");

        public async Task<List<DataChange>> PeekChanges(int count)
        {
            var values = await _redis.ListRangeAsync("DataChangeQueue", 0, count - 1);
            return values.Select(v => JsonSerializer.Deserialize<DataChange>(v!)!).ToList();
        }

        public async Task ClearQueue() => await _redis.KeyDeleteAsync("DataChangeQueue");

        // Additional helper methods for efficient querying
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

        private HashEntry[] GetHashEntries(Article entity)
        {
            var hash = new List<HashEntry>
            {
                new(ArticleProperty.Title, entity.Title),
                new(ArticleProperty.Content, entity.Content),
                new(ArticleProperty.Abstract, entity.Abstract),
                new(ArticleProperty.ArticleType, entity.ArticleType),
                new(ArticleProperty.ParentArticleId, entity.ParentArticleId?.ToString() ?? "0"),
                new(ArticleProperty.UserId, entity.UserId.ToString()),
                new(ArticleProperty.TopicId, entity.TopicId.ToString()),
                new(ArticleProperty.SortNumber, entity.SortNumber.ToString()),
                new(ArticleProperty.IsHidden, entity.IsHidden.ToString()),
                new(ArticleProperty.UpdateTime, entity.UpdateTime.Value.ToString("O")),
                new(ArticleProperty.CreateTime, entity.CreateTime.Value.ToString("O"))
            };
            return hash.ToArray();
        }
        private HashEntry GetHashEntry(Article entity, string propertyName)
        {
            RedisValue val = propertyName switch
            {
                ArticleProperty.Title => entity.Title,
                ArticleProperty.Content => entity.Content,
                ArticleProperty.Abstract => entity.Abstract,
                ArticleProperty.ArticleType => entity.ArticleType,
                ArticleProperty.ParentArticleId => entity.ParentArticleId?.ToString() ?? "0",
                ArticleProperty.UserId => entity.UserId.ToString(),
                ArticleProperty.TopicId => entity.TopicId.ToString(),
                ArticleProperty.SortNumber => entity.SortNumber.ToString(),
                ArticleProperty.IsHidden => entity.IsHidden.ToString(),
                ArticleProperty.UpdateTime => DateTime.UtcNow.ToString("O"),
                //ArticleProperty.CreateTime => entity.CreateTime.Value.ToString("O"),
                _ => throw new ArgumentException($"Invalid property name: {propertyName}")
            };
            return new HashEntry(propertyName, val);
        }
        private Article GetArticleFromHashEntry(HashEntry[] hash, int id)
        {
            var dict = hash.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

            var article = new Article
            {
                Id = id,
                Title = dict[ArticleProperty.Title],
                Content = dict[ArticleProperty.Content],
                Abstract = dict[ArticleProperty.Abstract],
                ArticleType = dict[ArticleProperty.ArticleType],
                ParentArticleId = dict.TryGetValue(ArticleProperty.ParentArticleId, out var parentId) && !string.IsNullOrEmpty(parentId)
                                  ? int.Parse(parentId)
                                  : null,
                UserId = int.Parse(dict[ArticleProperty.UserId]),
                TopicId = int.Parse(dict[ArticleProperty.TopicId]),
                SortNumber = int.Parse(dict[ArticleProperty.SortNumber]),
                IsHidden = int.Parse(dict[ArticleProperty.IsHidden]),
                CreateTime = DateTime.Parse(dict[ArticleProperty.CreateTime]),
                UpdateTime = DateTime.Parse(dict[ArticleProperty.UpdateTime])
            };
            return article;
        }
    }
}
