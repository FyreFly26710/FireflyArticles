using FF.Articles.Backend.Common.Requests;
using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Bases;
/// <summary>
/// TODO: Add one extra layer of abstraction to cache service
/// </summary>
public abstract class CacheService<TEntity, TContext>(TContext _context, ILogger<CacheService<TEntity, TContext>> _logger)
    : BaseService<TEntity, TContext>(_context, _logger)
    where TEntity : BaseEntity
    where TContext : DbContext
{
    public override Task<long> CreateAsync(TEntity entity) => base.CreateAsync(entity);
    public override Task<List<long>> CreateBatchAsync(List<TEntity> entities) => base.CreateBatchAsync(entities);
    public override Task<bool> DeleteAsync(long id) => base.DeleteAsync(id);
    public override Task<bool> DeleteBatchAsync(List<long> ids) => base.DeleteBatchAsync(ids);
    public override Task<bool> ExistsAsync(long id) => base.ExistsAsync(id);
    public override Task<List<TEntity>> GetAllAsync() => base.GetAllAsync();
    public override Task<List<TEntity>> GetAllAsync(List<long> ids) => base.GetAllAsync(ids);
    public override Task<PageResponse<TEntity>> GetAllAsync(PageRequest pageRequest) => base.GetAllAsync(pageRequest);
    public override async Task<TEntity> GetByIdAsync(long id) => await base.GetByIdAsync(id);
    public override Task<long> UpdateAsync(TEntity trackedEntity) => base.UpdateAsync(trackedEntity);
    public override Task<List<long>> UpdateBatchAsync(List<TEntity> entities) => base.UpdateBatchAsync(entities);
}
