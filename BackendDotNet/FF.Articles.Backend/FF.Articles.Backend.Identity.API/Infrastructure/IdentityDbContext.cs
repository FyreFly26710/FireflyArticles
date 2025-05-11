namespace FF.Articles.Backend.Identity.API.Infrastructure;
public class IdentityDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Auth");
        ConfigureUser(modelBuilder);


    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<User>(modelBuilder, [BaseProperty.CreateTime, BaseProperty.UpdateTime, BaseProperty.IsDelete]);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(u => u.UserAccount).HasMaxLength(32).IsRequired();
            entity.Property(u => u.UserPassword).HasMaxLength(32).IsRequired();
            entity.Property(u => u.UserEmail).HasMaxLength(128);
            entity.Property(u => u.UserName).HasMaxLength(32);
            entity.Property(u => u.UserAvatar).HasMaxLength(2048);
            entity.Property(u => u.UserProfile).HasMaxLength(2048);
            entity.Property(u => u.UserRole).HasMaxLength(32).IsRequired().HasDefaultValue("user");
        });
    }

}