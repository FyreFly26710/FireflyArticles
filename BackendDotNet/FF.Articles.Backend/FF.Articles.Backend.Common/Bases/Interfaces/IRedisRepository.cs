using System;
using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Common.Responses;
using StackExchange.Redis;

namespace FF.Articles.Backend.Common.Bases.Interfaces;

public interface IRedisRepository<TEntity> where TEntity : BaseEntity
{
    string EntityKey { get; }
    Task<int> GetNextIdAsync();
    Task<int> GetNextIdAsync(int count);
    string GetFieldKey(int id);
    Task<TEntity?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<int>> ExistIdsAsync();
    Task<List<TEntity>> GetByIdsAsync(List<int> ids);
    Task<List<TEntity>> GetAllAsync();
    Task<int> CreateAsync(TEntity entity);
    Task<List<TEntity>> CreateBatchAsync(List<TEntity> entities);
    Task<bool> UpdateAsync(TEntity entity);
    Task UpdateBatchAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteBatchAsync(List<int> ids);
    Task<Paged<TEntity>> GetPagedAsync(PageRequest pageRequest, List<TEntity>? entities = null);
    Task EnqueueChangeAsync(TEntity entity, ChangeType type, ITransaction? transaction = null);
    Task EnqueueChangesAsync(IEnumerable<TEntity> entities, ChangeType type, ITransaction? transaction = null);
    Task<int> GetQueueLength();
    Task<List<DataChange>> PeekChanges(int count);
    Task ClearQueue();
    // TODO: Implement this
    //Task<T> ExecuteWithTransactionAsync<T>(Func<ITransaction, Task<T>> action);
}