using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FF.Articles.Backend.Common.Bases;
public abstract class BaseService<TEntity, TContext>
    : IBaseService<TEntity>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    protected readonly IBaseRepository<TEntity, TContext> _repository;
    protected readonly ILogger<BaseService<TEntity, TContext>> _logger;
    public BaseService(IBaseRepository<TEntity, TContext> _repository, ILogger<BaseService<TEntity, TContext>> _logger)
    {
        this._repository = _repository;
        this._logger = _logger;
    }
    public virtual async Task<TEntity?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public virtual async Task<List<TEntity>> GetAllAsync() => await _repository.GetAllAsync();
    public virtual async Task<List<TEntity>> GetByIdsAsync(List<int> ids) => await _repository.GetByIdsAsync(ids);
    public virtual async Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest) => await _repository.GetAllAsync(pageRequest);

    public virtual async Task<int> CreateAsync(TEntity entity)
    {
        var id = await _repository.CreateAsync(entity);
        await _repository.SaveChangesAsync();
        return id;
    }
    public virtual async Task<bool> UpdateAsync(TEntity trackedEntity)
    {
        await _repository.UpdateAsync(trackedEntity);
        var result = await _repository.SaveChangesAsync();
        return result;
    }
    public virtual async Task<bool> DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveChangesAsync();
        return true;
    }

    // public virtual async Task<List<int>> CreateBatchAsync(List<TEntity> entities)
    // {
    //     var ids = await _repository.CreateBatchAsync(entities);
    //     await _repository.SaveChangesAsync();
    //     return ids;
    // }
    // public virtual async Task UpdateBatchAsync(List<TEntity> entities)
    // {
    //     await _repository.UpdateBatchAsync(entities);
    //     await _repository.SaveChangesAsync();
    // }
    // public virtual async Task<bool> DeleteBatchAsync(List<int> ids)
    // {
    //     await _repository.DeleteBatchAsync(ids);
    //     await _repository.SaveChangesAsync();
    //     return true;
    // }
}