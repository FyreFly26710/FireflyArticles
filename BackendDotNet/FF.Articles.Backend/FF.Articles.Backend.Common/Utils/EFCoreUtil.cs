using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Common.Utils;
public static class EFCoreUtil
{
    // public static IQueryable<T> ApplyPaging<T>(IQueryable<T> query, PageRequest pageRequest)
    // {

    //     if (!string.IsNullOrWhiteSpace(pageRequest.SortField))
    //     {
    //         query = pageRequest.SortOrder == SortOrderConstant.ASC
    //             ? query.OrderBy(e => EF.Property<object>(e, pageRequest.SortField))
    //             : query.OrderByDescending(e => EF.Property<object>(e, pageRequest.SortField));
    //     }

    //     return query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

    // }

    /// <summary>
    /// Keep all columns of the base entity (Id, CreateTime, UpdateTime, IsDelete). 
    /// Apply Soft delete and concurrency token
    /// </summary>
    public static void ConfigBaseEntity<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();  
            entity.Property(e => e.IsDelete).HasDefaultValue(0);
            //entity.Property(e => e.CreateTime).ValueGeneratedOnAdd();
            //entity.Property(e => e.UpdateTime).ValueGeneratedOnAddOrUpdate()
            //entity.Property(e => e.UpdateTime).IsConcurrencyToken();
        });
        // Soft delete
        modelBuilder.Entity<TEntity>().HasQueryFilter(u => u.IsDelete == 0);
    }
    /// <summary>
    /// Ignore UpdateTime, CreateTime, IsDelete columns. Only keep Id.
    /// </summary>
    public static void ConfigBasetEntityIgnoreOptions<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();  
        });
        modelBuilder.Entity<TEntity>().Ignore(e => e.UpdateTime);
        modelBuilder.Entity<TEntity>().Ignore(e => e.CreateTime);
        modelBuilder.Entity<TEntity>().Ignore(e => e.IsDelete);
    }
}


