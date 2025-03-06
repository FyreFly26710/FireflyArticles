using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Bases;
public interface IBaseService<TEntity, TContext> where TEntity : BaseEntity where TContext : DbContext
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetByIdAsTrackingAsync(int id);
    Task<int> SaveAsync();
    Task<List<TEntity>> GetAllAsync(List<int> ids);
    Task<List<TEntity>> GetAllAsync();
    Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest);
    Task<Paged<TEntity>> GetPagedAsync(IQueryable<TEntity> query, PageRequest pageRequest);
    IQueryable<TEntity> ApplyPageRequestQuery(IQueryable<TEntity> query,PageRequest pageRequest);
    IQueryable<TEntity> GetQueryable();
    Task<int> CreateAsync(TEntity entity);
    Task<List<int>> CreateBatchAsync(List<TEntity> entities);
    Task<int> UpdateAsync(TEntity entity);
    Task<List<int>> UpdateBatchAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(int id);
    Task<bool> HardDeleteAsync(int id);
    Task<bool> DeleteBatchAsync(List<int> ids);
    Task<bool> ExistsAsync(int id);

}