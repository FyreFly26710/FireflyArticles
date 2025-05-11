namespace FF.Articles.Backend.Common.Utils;
public static class EFCoreUtil
{
    public static void ConfigBaseEntity<TEntity>(ModelBuilder modelBuilder, List<BaseProperty> properties) where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            if (properties.Contains(BaseProperty.IsDelete))
                entity.Property(e => e.IsDelete).HasDefaultValue(0);
        });
        // Soft delete
        if (properties.Contains(BaseProperty.IsDelete))
            modelBuilder.Entity<TEntity>().HasQueryFilter(u => u.IsDelete == 0);

        if (!properties.Contains(BaseProperty.CreateTime))
            modelBuilder.Entity<TEntity>().Ignore(e => e.CreateTime);
        if (!properties.Contains(BaseProperty.UpdateTime))
            modelBuilder.Entity<TEntity>().Ignore(e => e.UpdateTime);
        if (!properties.Contains(BaseProperty.IsDelete))
            modelBuilder.Entity<TEntity>().Ignore(e => e.IsDelete);
    }
}


public enum BaseProperty
{
    // Id,
    CreateTime,
    UpdateTime,
    IsDelete
}