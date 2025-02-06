using FF.Articles.Backend.Common.Exceptions;
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
public class BaseService<TEntity,TContext> : IBaseService<TEntity, TContext> where TEntity : BaseEntity where TContext : DbContext
{
    protected readonly TContext _context;
    private readonly ILogger<BaseService<TEntity, TContext>> _logger;

    public BaseService(TContext context, ILogger<BaseService<TEntity, TContext>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TEntity> GetByIdAsync(long id)
    {
        TEntity? entity = await _context.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            _logger.LogError($"Entity {typeof(TEntity)} with id {id} not found");
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR);
        }
        return entity;
    }

    public async Task<List<TEntity>> GetAllAsync() => await _context.Set<TEntity>().ToListAsync();
    public async Task<List<TEntity>> GetAllIncludeDeleteAsync() => await _context.Set<TEntity>().IgnoreQueryFilters().ToListAsync();
    public IQueryable<TEntity> AsQueryable() => _context.Set<TEntity>().AsQueryable();
    public async Task<long> CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR);
        }

        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<List<long>> CreateBatchAsync(List<TEntity> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR);
        }

        await _context.Set<TEntity>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        return entities.Select(e => e.Id).ToList();
    }

    public async Task<List<TEntity>> GetByIdsAsync(List<long> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return new List<TEntity>();
        }

        return await _context.Set<TEntity>()
                             .Where(e => ids.Contains(e.Id))
                             .ToListAsync();
    }
    public async Task<PageResponse<TEntity>> GetPagedList(PageRequest pageRequest)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        // Apply sorting if a SortField is provided
        if (!string.IsNullOrWhiteSpace(pageRequest.SortField))
        {
            query = pageRequest.SortOrder == SortOrder.ASC
                ? query.OrderBy(e => EF.Property<object>(e, pageRequest.SortField))
                : query.OrderByDescending(e => EF.Property<object>(e, pageRequest.SortField));
        }

        // Get total count before applying pagination
        long totalCount = await query.LongCountAsync();

        // Apply pagination
        var data = await query
            .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync();

        return new PageResponse<TEntity>(pageRequest.PageNumber, pageRequest.PageSize, totalCount, data);
    }
    public async Task<long> UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR);
        }

        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<List<long>> UpdateBatchAsync(List<TEntity> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR);
        }

        _context.Set<TEntity>().UpdateRange(entities);
        await _context.SaveChangesAsync();

        return entities.Select(e => e.Id).ToList();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteByIds(List<long> ids)
    {
        var entities = await GetByIdsAsync(ids);

        if (!entities.Any())
        {
            return false;
        }

        _context.Set<TEntity>().RemoveRange(entities);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ExistsAsync(long id) => await _context.Set<TEntity>().AnyAsync(e => e.Id == id);
}