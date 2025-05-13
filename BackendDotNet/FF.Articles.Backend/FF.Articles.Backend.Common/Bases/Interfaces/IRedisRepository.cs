// namespace FF.Articles.Backend.Common.Bases.Interfaces;

// [Obsolete("This is not used anymore, only use redis for caching")]
// public interface IRedisRepository<TEntity> where TEntity : BaseEntity
// {
//     string EntityKey { get; }
//     // Task<string> GetNextIdAsync();
//     // Task<string> GetNextIdAsync(int count);
//     string GetFieldKey(long id);
//     Task<TEntity?> GetByIdAsync(long id);
//     Task<bool> ExistsAsync(long id);
//     Task<List<long>> ExistIdsAsync();
//     Task<List<TEntity>> GetByIdsAsync(List<long> ids);
//     Task<List<TEntity>> GetAllAsync();
//     Task<long> CreateAsync(TEntity entity);
//     Task<List<TEntity>> CreateBatchAsync(List<TEntity> entities);
//     Task<bool> UpdateAsync(TEntity entity);
//     Task UpdateBatchAsync(List<TEntity> entities);
//     Task<bool> DeleteAsync(long id);
//     Task<bool> DeleteBatchAsync(List<long> ids);
//     Task<Paged<TEntity>> GetPagedAsync(PageRequest pageRequest, List<TEntity>? entities = null);
//     Task EnqueueChangeAsync(TEntity entity, ChangeType type, ITransaction? transaction = null);
//     Task EnqueueChangesAsync(IEnumerable<TEntity> entities, ChangeType type, ITransaction? transaction = null);
//     Task<int> GetQueueLength();
//     Task<List<DataChange>> PeekChanges(int count);
//     Task ClearQueue();
//     // TODO: Implement this
//     //Task<T> ExecuteWithTransactionAsync<T>(Func<ITransaction, Task<T>> action);
// }