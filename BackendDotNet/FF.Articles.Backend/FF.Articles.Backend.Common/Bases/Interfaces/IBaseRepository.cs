using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Common.Bases.Interfaces;
public interface IBaseRepository<TEntity, TContext>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    Task<TEntity?> GetByIdAsync(long id, bool asTracking = false);
    Task<List<TEntity>> GetByIdsAsync(List<long> ids, bool asTracking = false);
    Task<List<TEntity>> GetAllAsync(bool asTracking = false);
    Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest);
    Task<Paged<TEntity>> GetPagedFromQueryAsync(IQueryable<TEntity> query, PageRequest pageRequest);
    IQueryable<TEntity> ApplyPageRequestQuery(IQueryable<TEntity> query, PageRequest pageRequest);
    IQueryable<TEntity> GetQueryable();
    Task<long> CreateAsync(TEntity entity);
    Task<List<TEntity>> CreateBatchAsync(List<TEntity> entities);
    /// <summary>
    /// Fetch the original entity from DB <br/>
    /// Attach the incoming entity <br/>
    /// Call UpdateChecker to check if any property has actually changed <br/>
    /// </summary>
    Task<bool> UpdateAsync(TEntity entity, TEntity? dbEntity = null);
    /// <summary>
    /// Pass in the tracked entity (to be updated) and the db entity (original from db) <br/>
    /// Check if any property has actually changed <br/>
    /// If so, update the UpdateTime property, set the entry.property state to modified and return true <br/>
    /// Otherwise, reset the entry state to unchanged and return false
    /// </summary>
    bool UpdateChecker(TEntity trackedEntity, TEntity dbEntity);
    //Task UpdateBatchAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(long id);
    Task<bool> HardDeleteAsync(long id);
    Task<bool> DeleteBatchAsync(List<long> ids);
    Task<bool> ExistsAsync(long id);
    Task<bool> SaveChangesAsync();
}

