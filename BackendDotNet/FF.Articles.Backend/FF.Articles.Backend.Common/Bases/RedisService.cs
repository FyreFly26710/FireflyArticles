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


    public Task<int> CreateAsync(TEntity entity) => _redisRepository.CreateAsync(entity);

    public Task<bool> DeleteAsync(int id) => _redisRepository.DeleteAsync(id);

    public Task<List<TEntity>> GetAllAsync() => _redisRepository.GetAllAsync();

    public Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest) => _redisRepository.GetPagedAsync(pageRequest);


    public Task<TEntity?> GetByIdAsync(int id) => _redisRepository.GetByIdAsync(id);

    public Task<List<TEntity>> GetByIdsAsync(List<int> ids) => _redisRepository.GetByIdsAsync(ids);

    public Task<bool> UpdateAsync(TEntity entity) => _redisRepository.UpdateAsync(entity);

    // public Task<bool> DeleteBatchAsync(List<int> ids) => _redisRepository.DeleteBatchAsync(ids);
    // public Task<int> CreateBatchAsync(List<TEntity> entities) => _redisRepository.CreateBatchAsync(entities);
    // public Task UpdateBatchAsync(List<TEntity> entities) => _redisRepository.UpdateBatchAsync(entities);

}
