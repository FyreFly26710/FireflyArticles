

using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Common.Bases;
public abstract class BaseRepository<TEntity, TContext>
    : IBaseRepository<TEntity, TContext>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    protected readonly TContext _context;
    public BaseRepository(TContext _context)
    {
        this._context = _context;
    }
    /// <summary>
    /// No Tracking by default
    /// </summary>
    public virtual IQueryable<TEntity> GetQueryable() => _context.Set<TEntity>().AsQueryable();
    public virtual async Task<bool> ExistsAsync(int id) => await _context.Set<TEntity>().AnyAsync(e => e.Id == id);

    public virtual async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    #region single CRUD operations
    public virtual TEntity? GetById(int id) => _context.Set<TEntity>().Find(id);
    public virtual async Task<TEntity?> GetByIdAsync(int id, bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (asTracking)
            query = query.AsTracking();
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<int> CreateAsync(TEntity entity)
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity));

        if (entityType?.FindProperty(nameof(BaseEntity.CreateTime)) != null)
        {
            if (entity.CreateTime == null)
                entity.CreateTime = DateTime.UtcNow;
        }

        if (entityType?.FindProperty(nameof(BaseEntity.UpdateTime)) != null)
        {
            if (entity.UpdateTime == null)
                entity.UpdateTime = DateTime.UtcNow;
        }

        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }
    public virtual async Task<int> UpdateModifiedAsync(TEntity trackedEntity)
    {
        var entry = _context.Entry(trackedEntity);
        // Check if any properties have been modified
        if (entry.State == EntityState.Modified || entry.Properties.Any(p => p.IsModified))
        {
            if (entry.Metadata.FindProperty(nameof(BaseEntity.UpdateTime)) != null)
            {
                entry.Property(nameof(BaseEntity.UpdateTime)).CurrentValue = DateTime.UtcNow;
            }
        }
        // await _context.SaveChangesAsync();
        return trackedEntity.Id;
    }
    public virtual async Task<int> UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        var id = await UpdateModifiedAsync(entity);
        return id;
    }


    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id, true);
        if (entity == null)
            return true;

        var entityType = _context.Model.FindEntityType(typeof(TEntity));

        if (entityType?.FindProperty(nameof(BaseEntity.IsDelete)) != null)
        {
            entity.IsDelete = 1;
        }
        else
        {
            _context.Remove(entity);
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> HardDeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id, true);
        if (entity == null)
            return false;

        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion



    #region batch CRUD operations

    public virtual async Task<List<TEntity>> GetAllAsync(bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (asTracking)
            query = query.AsTracking();
        return await query.ToListAsync();
    }
    public virtual async Task<List<TEntity>> GetByIdsAsync(List<int> ids, bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (asTracking)
            query = query.AsTracking();
        return await query.Where(e => ids.Contains(e.Id)).ToListAsync();
    }
    public virtual async Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest) =>
        await GetPagedFromQueryAsync(_context.Set<TEntity>(), pageRequest);

    public virtual async Task<Paged<TEntity>> GetPagedFromQueryAsync(IQueryable<TEntity> query, PageRequest pageRequest)
    {
        // Get total count before applying pagination
        int totalCount = await query.CountAsync();
        // Apply pagination
        query = ApplyPageRequestQuery(query, pageRequest);
        // Execuate query
        var data = await query.ToListAsync();

        return new Paged<TEntity>(pageRequest.PageNumber, pageRequest.PageSize, totalCount, data);
    }
    public virtual IQueryable<TEntity> ApplyPageRequestQuery(IQueryable<TEntity> query, PageRequest pageRequest)
    {
        // Apply sorting if a SortField is provided
        if (!string.IsNullOrWhiteSpace(pageRequest.SortField))
        {
            var propertyInfo = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.Equals(pageRequest.SortField, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo != null)
            {
                query = pageRequest.SortOrder == SortOrderConstant.ASC
                    ? query.OrderBy(e => EF.Property<object>(e, propertyInfo.Name))
                    : query.OrderByDescending(e => EF.Property<object>(e, propertyInfo.Name));
            }
        }

        // Apply pagination
        return query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);
    }

    public virtual async Task<List<int>> CreateBatchAsync(List<TEntity> entities)
    {
        if (entities.Count == 0)
            return new List<int>();

        var entityType = _context.Model.FindEntityType(typeof(TEntity));

        if (entityType?.FindProperty(nameof(BaseEntity.CreateTime)) != null)
        {
            foreach (var entity in entities)
            {
                if (entity.CreateTime == null)
                    entity.CreateTime = DateTime.UtcNow;
            }
        }

        if (entityType?.FindProperty(nameof(BaseEntity.UpdateTime)) != null)
        {
            foreach (var entity in entities)
            {
                if (entity.UpdateTime == null)
                    entity.UpdateTime = DateTime.UtcNow;
            }
        }

        await _context.Set<TEntity>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities.Select(e => e.Id).ToList();
    }
    public virtual async Task UpdateBatchAsync(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            await UpdateModifiedAsync(entity);
        }
        await _context.SaveChangesAsync();
    }
    public virtual async Task<bool> DeleteBatchAsync(List<int> ids)
    {
        var entities = await _context.Set<TEntity>().AsTracking()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();
        foreach (var entity in entities)
            entity.IsDelete = 1;
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

}
