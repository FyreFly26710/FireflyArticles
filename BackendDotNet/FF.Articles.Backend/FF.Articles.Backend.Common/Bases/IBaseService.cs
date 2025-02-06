using FF.Articles.Backend.Common.Requests;
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
    Task<TEntity> GetByIdAsync(long id);
    Task<List<TEntity>> GetByIdsAsync(List<long> ids);
    Task<List<TEntity>> GetAllAsync();
    Task<List<TEntity>> GetAllIncludeDeleteAsync();
    Task<PageResponse<TEntity>> GetPagedList(PageRequest pageRequest);
    IQueryable<TEntity> AsQueryable();
    Task<long> CreateAsync(TEntity entity);
    Task<List<long>> CreateBatchAsync(List<TEntity> entities);
    Task<long> UpdateAsync(TEntity entity);
    Task<List<long>> UpdateBatchAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(long id);
    Task<bool> DeleteByIds(List<long> ids);
    Task<bool> ExistsAsync(long id);

}