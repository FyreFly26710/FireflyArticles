using FF.Articles.Backend.Common.Utils;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Infrastructure;
public class ContentsDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ArticleTag> ArticleTags { get; set; }

    public ContentsDbContext(DbContextOptions<ContentsDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Contents");

        ConfigureArticle(modelBuilder);
        ConfigureTopic(modelBuilder);
        ConfigureTag(modelBuilder);
        ConfigureArticleTag(modelBuilder);


    }

    private void ConfigureArticle(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<Article>(modelBuilder);

        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("Article");
            entity.Property(e => e.Title).HasMaxLength(1000).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Abstraction).HasMaxLength(4000).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Content).HasColumnType("NVARCHAR(MAX)").HasDefaultValue("");
            entity.Property(e => e.IsHidden).IsRequired().HasDefaultValue(0);

        });
    }
    private void ConfigureTopic(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<Topic>(modelBuilder);

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("Topic");
            entity.Property(e => e.Title).HasMaxLength(1000).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Abstraction).HasMaxLength(4000).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Content).HasColumnType("NVARCHAR(MAX)").HasDefaultValue("");
            entity.Property(e => e.IsHidden).IsRequired().HasDefaultValue(0);

        });
    }
    private void ConfigureTag(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBasetEntityIgnoreOptions<Tag>(modelBuilder);

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.Property(e => e.TagName).HasMaxLength(255).IsRequired();
        });
    }
    private void ConfigureArticleTag(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBasetEntityIgnoreOptions<ArticleTag>(modelBuilder);

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.ToTable("ArticleTag");
        });
    }
}