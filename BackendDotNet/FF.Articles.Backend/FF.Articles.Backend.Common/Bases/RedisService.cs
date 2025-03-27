using System;
using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Common.Responses;
using Microsoft.Extensions.Logging;

namespace FF.Articles.Backend.Common.Bases;

public class RedisService<TEntity> : IBaseService<TEntity>
where TEntity : BaseEntity
{
    protected readonly IRedisRepository<TEntity> _redisRepository;
    protected readonly ILogger<RedisService<TEntity>> _logger;
    public RedisService(IRedisRepository<TEntity> redisRepository, ILogger<RedisService<TEntity>> logger = null)
    {
        _redisRepository = redisRepository;
        _logger = logger;
    }


    public virtual Task<long> CreateAsync(TEntity entity) => _redisRepository.CreateAsync(entity);

    public virtual Task<bool> DeleteAsync(long id) => _redisRepository.DeleteAsync(id);

    public virtual Task<List<TEntity>> GetAllAsync() => _redisRepository.GetAllAsync();

    public virtual Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest) => _redisRepository.GetPagedAsync(pageRequest);


    public virtual Task<TEntity?> GetByIdAsync(long id) => _redisRepository.GetByIdAsync(id);

    public virtual Task<List<TEntity>> GetByIdsAsync(List<long> ids) => _redisRepository.GetByIdsAsync(ids);

    public virtual Task<bool> UpdateAsync(TEntity entity) => _redisRepository.UpdateAsync(entity);

    // public Task<bool> DeleteBatchAsync(List<int> ids) => _redisRepository.DeleteBatchAsync(ids);
    // public Task<int> CreateBatchAsync(List<TEntity> entities) => _redisRepository.CreateBatchAsync(entities);
    // public Task UpdateBatchAsync(List<TEntity> entities) => _redisRepository.UpdateBatchAsync(entities);

}
