using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public partial class ArticleRedisRepository
{
    private readonly IDatabase _redis;
    private readonly ILogger<ArticleRedisRepository> _logger;


    // public async Task<int> GetNextIdAsync()
    // {
    //     return (int)await _redis.StringIncrementAsync(ID_COUNTER);
    // }

    // public async Task<int> GetNextIdAsync(int count)
    // {
    //     long startId = await _redis.StringIncrementAsync(ID_COUNTER, count);
    //     return (int)(startId - count + 1);
    // }

    public string GetFieldKey(long id) => $"{KEY_PREFIX}{id}";

    public async Task<bool> ExistsAsync(long id)
    {
        return await _redis.KeyExistsAsync(GetFieldKey(id));
    }

    public async Task<List<long>> ExistIdsAsync()
    {
        var members = await _redis.SetMembersAsync(ARTICLE_IDS_KEY);
        return members.Select(m => (long)m).ToList();
    }

    public async Task<Article?> GetByIdAsync(long id)
    {
        if (id == 0)
            return null;
        var key = GetFieldKey(id);
        var hash = await _redis.HashGetAllAsync(key);

        if (!hash.Any()) return null;

        return GetArticleFromHashEntry(hash, id);
    }


    public async Task<List<Article>> GetByIdsAsync(List<long> ids)
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

    public async Task<long> CreateAsync(Article entity)
    {
        if (entity.Id == 0)
            entity.Id = EntityUtil.GenerateSnowflakeId();
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

    public async Task<List<Article>> CreateBatchAsync(List<Article> entities)
    {
        var tasks = new List<Task>();

        foreach (var entity in entities)
        {
            if (string.IsNullOrEmpty(entity.Id.ToString()))
                entity.Id = EntityUtil.GenerateSnowflakeId();
            entity.CreateTime ??= DateTime.UtcNow;
            entity.UpdateTime ??= DateTime.UtcNow;

            var key = GetFieldKey(entity.Id);
            var hash = GetHashEntries(entity);

            tasks.Add(_redis.HashSetAsync(key, hash));
            tasks.Add(_redis.SetAddAsync($"{TOPIC_INDEX}{entity.TopicId}", entity.Id));
            if (entity.ParentArticleId != null)
            {
                tasks.Add(_redis.SetAddAsync($"{PARENT_INDEX}{entity.ParentArticleId}", entity.Id));
            }
        }

        await Task.WhenAll(tasks);
        await EnqueueChangesAsync(entities, ChangeType.Create);
        return entities;
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

    public async Task UpdateBatchAsync(List<Article> entities)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(long id)
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


    public async Task<bool> DeleteBatchAsync(List<long> ids)
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


}
