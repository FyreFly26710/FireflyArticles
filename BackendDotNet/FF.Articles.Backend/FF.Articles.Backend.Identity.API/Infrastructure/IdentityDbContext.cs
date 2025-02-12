using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
        //modelBuilder.HasDefaultSchema("Identity");
        ConfigureUser(modelBuilder);


    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<User>(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(u => u.UserAccount).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UserPassword).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UserName).HasMaxLength(255);
            entity.Property(u => u.UserAvatar).HasMaxLength(1024);
            entity.Property(u => u.UserProfile).HasMaxLength(512);
            entity.Property(u => u.UserRole).HasMaxLength(50).IsRequired().HasDefaultValue("user"); 

        });
    }

}