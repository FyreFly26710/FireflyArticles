using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Common.Bases;
public interface IBaseRepository<TEntity, TContext>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    TEntity? GetById(int id);
    Task<TEntity?> GetByIdAsync(int id, bool asTracking = false);
    Task<int> SaveAsync();
    Task<List<TEntity>> GetByIdsAsync(List<int> ids, bool asTracking = false);
    Task<List<TEntity>> GetAllAsync(bool asTracking = false);
    Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest);
    Task<Paged<TEntity>> GetPagedFromQueryAsync(IQueryable<TEntity> query, PageRequest pageRequest);
    IQueryable<TEntity> ApplyPageRequestQuery(IQueryable<TEntity> query, PageRequest pageRequest);
    IQueryable<TEntity> GetQueryable();
    Task<int> CreateAsync(TEntity entity);
    Task<List<int>> CreateBatchAsync(List<TEntity> entities);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> UpdateModifiedAsync(TEntity entity);
    Task UpdateBatchAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(int id);
    Task<bool> HardDeleteAsync(int id);
    Task<bool> DeleteBatchAsync(List<int> ids);
    Task<bool> ExistsAsync(int id);
}

