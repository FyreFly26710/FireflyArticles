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
    public virtual async Task<bool> ExistsAsync(long id) => await _context.Set<TEntity>().AnyAsync(e => e.Id == id);
    public virtual async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

    public virtual async Task<TEntity?> GetByIdAsync(long id, bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (asTracking)
            query = query.AsTracking();
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }
    public virtual async Task<List<TEntity>> GetAllAsync(bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (asTracking)
            query = query.AsTracking();
        return await query.ToListAsync();
    }
    public virtual async Task<List<TEntity>> GetByIdsAsync(List<long> ids, bool asTracking = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        if (asTracking)
            query = query.AsTracking();
        return await query.Where(e => ids.Contains(e.Id)).ToListAsync();
    }
    public virtual async Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest)
    {
        return await GetPagedFromQueryAsync(_context.Set<TEntity>(), pageRequest);
    }


    public virtual async Task<long> CreateAsync(TEntity entity)
    {
        if (entity.Id == 0)
            entity.Id = EntityUtil.GenerateSnowflakeId();

        var entityType = _context.Model.FindEntityType(typeof(TEntity));

        if (entityType?.FindProperty(nameof(BaseEntity.CreateTime)) != null)
            entity.CreateTime ??= DateTime.UtcNow;

        if (entityType?.FindProperty(nameof(BaseEntity.UpdateTime)) != null)
            entity.UpdateTime ??= DateTime.UtcNow;

        await _context.Set<TEntity>().AddAsync(entity);
        return entity.Id;
    }
    public virtual async Task<List<TEntity>> CreateBatchAsync(List<TEntity> entities)
    {
        if (entities.Count == 0)
            return new List<TEntity>();

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
        foreach (var entity in entities)
        {
            if (entity.Id == 0)
                entity.Id = EntityUtil.GenerateSnowflakeId();
        }
        await _context.Set<TEntity>().AddRangeAsync(entities);
        //await _context.SaveChangesAsync();
        return entities;
    }

    /// <summary>
    /// Pass in the tracked entity (to be updated) and the db entity (original from db) <br/>
    /// Check if any property has actually changed <br/>
    /// If so, update the UpdateTime property, set the entry.property state to modified and return true <br/>
    /// Otherwise, reset the entry state to unchanged and return false
    /// </summary>
    /// <param name="trackedEntity"></param>
    /// <param name="dbEntity"></param>
    /// <returns></returns>
    public virtual bool UpdateChecker(TEntity trackedEntity, TEntity dbEntity)
    {
        var entry = _context.Entry(trackedEntity);
        var dbEntry = _context.Entry(dbEntity);

        bool hasRealChanges = false;

        foreach (var property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;
            var dbProperty = dbEntry.Property(propertyName);

            // Check if the property has actually changed
            if (!Equals(dbProperty.CurrentValue, property.CurrentValue))
            {
                property.IsModified = true; // Mark only changed properties as modified
                hasRealChanges = true;
            }
        }

        // If changes exist, update UpdateTime
        if (hasRealChanges && entry.Metadata.FindProperty(nameof(BaseEntity.UpdateTime)) != null)
        {
            entry.Property(nameof(BaseEntity.UpdateTime)).CurrentValue = DateTime.UtcNow;
            entry.Property(nameof(BaseEntity.UpdateTime)).IsModified = true;
        }
        if (!hasRealChanges)
        {
            entry.State = EntityState.Unchanged;
        }

        return hasRealChanges;
    }

    /// <summary>
    /// Fetch the original entity from DB <br/>
    /// Attach the incoming entity <br/>
    /// Call UpdateChecker to check if any property has actually changed <br/>
    /// </summary>
    public virtual async Task<bool> UpdateAsync(TEntity entity, TEntity? dbEntity = null)
    {
        // Fetch original entity from DB
        if (dbEntity == null)
            dbEntity = await GetByIdAsync(entity.Id);

        if (dbEntity == null)
            throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");

        // Attach the incoming entity
        _context.Set<TEntity>().Attach(entity);

        // Check for actual changes vs. DB entity
        bool hasChanges = UpdateChecker(entity, dbEntity);

        return hasChanges;
    }
    //public virtual async Task UpdateBatchAsync(List<TEntity> entities)
    //{
    //    var dbEntities = await GetByIdsAsync(entities.Select(e => e.Id).ToList());
    //    foreach (var entity in entities)
    //    {
    //        await UpdateAsync(entity, dbEntities.FirstOrDefault(e => e.Id == entity.Id));
    //    }
    //}

    public virtual async Task<bool> DeleteAsync(long id)
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
        return true;
    }

    public virtual async Task<bool> DeleteBatchAsync(List<long> ids)
    {
        var entities = await GetByIdsAsync(ids, true);
        var entityType = _context.Model.FindEntityType(typeof(TEntity));

        if (entityType?.FindProperty(nameof(BaseEntity.IsDelete)) != null)
        {
            foreach (var entity in entities)
                entity.IsDelete = 1;
        }
        else
        {
            _context.RemoveRange(entities);
        }
        return true;
    }

    public virtual async Task<bool> HardDeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id, true);
        if (entity == null)
            return true;

        _context.Remove(entity);
        return true;
    }

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

}
