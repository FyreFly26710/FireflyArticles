using FF.Articles.Backend.Common.Utils;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Article.API.Infrastructure;
public class ArticleDbContext : DbContext
{
    public DbSet<Models.Entities.Article> Users { get; set; }

    public ArticleDbContext(DbContextOptions<ArticleDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);


    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>().HasQueryFilter(u => u.IsDelete == 0);//only query the data that is not deleted
        EFCoreUtil.ConfigBasetEntity<User>(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.Property(u => u.UserAccount).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UserPassword).HasMaxLength(255).IsRequired();
            entity.Property(u => u.UserName).HasMaxLength(255);
            entity.Property(u => u.UserAvatar).HasMaxLength(1024);
            entity.Property(u => u.UserProfile).HasMaxLength(512);
            entity.Property(u => u.UserRole).HasMaxLength(50).IsRequired().HasDefaultValue("user");

        });
    }

}