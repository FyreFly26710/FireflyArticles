using System;
using System.Text.Json;
using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FF.Articles.Backend.Common.Bases;
public abstract class RedisRepository<TEntity> : IRedisRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    protected readonly IDatabase _redis;
    private readonly bool _hasTime;
    private readonly ILogger<RedisRepository<TEntity>> _logger;
    protected readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    protected RedisRepository(IConnectionMultiplexer redis, ILogger<RedisRepository<TEntity>> logger, bool hasTime = true)
    {
        _redis = redis.GetDatabase();
        _logger = logger;
        _hasTime = hasTime;
    }
    public string EntityKey { get => typeof(TEntity).Name; }
    // public async Task<int> GetNextIdAsync() => (int)await _redis.StringIncrementAsync($"id:{EntityKey}");

    // public async Task<int> GetNextIdAsync(int count)
    // {
    //     long startId = await _redis.StringIncrementAsync($"id:{EntityKey}", count);
    //     long baseId = startId - count + 1;
    //     return (int)baseId;
    // }
    
    public string GetFieldKey(long id) => id.ToString();
    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await _redis.HashExistsAsync(EntityKey, GetFieldKey(id));
    }
    // Get all ids in the entity
    public virtual async Task<List<long>> ExistIdsAsync()
    {
        var ids = await _redis.HashKeysAsync(EntityKey);
        return ids.Select(id => (long)id).ToList();
    }
    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        var value = await _redis.HashGetAsync(EntityKey, GetFieldKey(id));
        var entity = value.HasValue ? JsonSerializer.Deserialize<TEntity>(value!) : null;
        return entity;
    }
    public virtual async Task<List<TEntity>> GetByIdsAsync(List<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return [];

        var redisFields = ids.Select(id => (RedisValue)GetFieldKey(id)).ToArray();

        // Fetch multiple fields at once
        var redisValues = await _redis.HashGetAsync(EntityKey, redisFields);

        // Deserialize and filter out missing/null values
        var entities = redisValues
            .Where(v => v.HasValue)
            .Select(v => JsonSerializer.Deserialize<TEntity>(v!)!)
            .ToList();

        return entities;
    }


    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        var entries = await _redis.HashGetAllAsync(EntityKey);
        var entities = entries
            .Where(e => e.Value.HasValue)
            .Select(e => JsonSerializer.Deserialize<TEntity>(e.Value!)!)
            .ToList();
        return entities;
    }

    public virtual async Task<long> CreateAsync(TEntity entity)
    {

        if (entity.Id == 0)
            entity.Id = EntityUtil.GenerateSnowflakeId();
        if (_hasTime)
        {
            entity.CreateTime ??= DateTime.UtcNow;
            entity.UpdateTime ??= DateTime.UtcNow;
        }

        var json = JsonSerializer.Serialize(entity, _options);

        await _redis.HashSetAsync(EntityKey, GetFieldKey(entity.Id), json);
        await EnqueueChangeAsync(entity, ChangeType.Create);

        return entity.Id;

    }

    public async Task<List<TEntity>> CreateBatchAsync(List<TEntity> entities)
    {
        var entries = new List<HashEntry>();
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            if (entity.Id == 0)
                entity.Id = EntityUtil.GenerateSnowflakeId();
            if (_hasTime)
            {
                entity.CreateTime ??= DateTime.UtcNow;
                entity.UpdateTime ??= DateTime.UtcNow;
            }

            var json = JsonSerializer.Serialize(entity, _options);
            entries.Add(new HashEntry(GetFieldKey(entity.Id), json));
        }

        await _redis.HashSetAsync(EntityKey, entries.ToArray());
        await EnqueueChangesAsync(entities, ChangeType.Create);
        return entities;

    }


    public virtual async Task<bool> UpdateAsync(TEntity entity)
    {

        if (!await ExistsAsync(entity.Id))
            throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");

        if (_hasTime)
        {
            entity.UpdateTime ??= DateTime.UtcNow;
        }
        var json = JsonSerializer.Serialize(entity, _options);
        await EnqueueChangeAsync(entity, ChangeType.Update);
        var result = await _redis.HashSetAsync(EntityKey, GetFieldKey(entity.Id), json);
        return result;

    }

    public virtual async Task UpdateBatchAsync(List<TEntity> entities)
    {
        if (entities == null || !entities.Any())
            return;

        // First, verify all entities exist
        var ids = entities.Select(e => e.Id).ToList();
        var existingIds = await ExistIdsAsync();
        var missingIds = ids.Except(existingIds).ToList();
        if (missingIds.Any())
        {
            throw new KeyNotFoundException($"Entities with IDs {string.Join(", ", missingIds)} not found.");
        }

        // Prepare batch update
        var entries = new List<HashEntry>();
        foreach (var entity in entities)
        {
            if (_hasTime)
            {
                entity.UpdateTime ??= DateTime.UtcNow;
            }
            var json = JsonSerializer.Serialize(entity, _options);
            entries.Add(new HashEntry(GetFieldKey(entity.Id), json));
        }

        // Perform batch update in Redis
        await _redis.HashSetAsync(EntityKey, entries.ToArray());

        // Enqueue all changes
        await EnqueueChangesAsync(entities, ChangeType.Update);
    }

    public virtual async Task<bool> DeleteAsync(long id)
    {
        var entity = new TEntity { Id = id };
        await EnqueueChangeAsync(entity, ChangeType.Delete);
        var result = await _redis.HashDeleteAsync(EntityKey, GetFieldKey(id));
        return result;
    }

    public virtual async Task<bool> DeleteBatchAsync(List<long> ids)
    {
        var keys = ids.Select(id => (RedisValue)GetFieldKey(id)).ToArray();
        await _redis.HashDeleteAsync(EntityKey, keys);
        await EnqueueChangesAsync(ids.Select(id => new TEntity { Id = id }), ChangeType.Delete);
        return true;
    }



    public virtual async Task<Paged<TEntity>> GetPagedAsync(PageRequest pageRequest, List<TEntity>? entities = null)
    {
        var all = entities ?? await GetAllAsync();
        var total = all.Count;
        var items = all
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToList();

        return new Paged<TEntity>(pageRequest.PageNumber, pageRequest.PageSize, total, items);
    }

    //public async Task<T> ExecuteWithTransactionAsync<T>(Func<ITransaction, Task<T>> action)
    //{
    //    var tran = _redis.CreateTransaction();
    //    try
    //    {
    //        var result = await action(tran);
    //        await tran.ExecuteAsync();
    //        return result;
    //    }
    //    catch
    //    {
    //        throw;
    //    }
    //}

    public virtual async Task EnqueueChangeAsync(TEntity entity, ChangeType type, ITransaction? transaction = null)
    {
        await EnqueueChangesAsync([entity], type, transaction);
    }

    public virtual async Task EnqueueChangesAsync(IEnumerable<TEntity> entities, ChangeType type, ITransaction? transaction = null)
    {
        var changes = entities.Select(entity => new DataChange
        {
            FullName = typeof(TEntity).FullName!,
            EntityType = EntityKey,
            Id = entity.Id,
            ChangeType = type,
            PayloadJson = type == ChangeType.Delete ? null : JsonSerializer.Serialize(entity, _options)
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