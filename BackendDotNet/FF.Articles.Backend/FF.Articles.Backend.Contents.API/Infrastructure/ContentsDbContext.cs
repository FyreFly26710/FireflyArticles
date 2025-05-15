using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        EFCoreUtil.ConfigBaseEntity<Article>(modelBuilder, [BaseProperty.CreateTime, BaseProperty.UpdateTime, BaseProperty.IsDelete]);

        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("Article");
            entity.Property(e => e.Title).HasMaxLength(256).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Abstract).HasMaxLength(1024).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Content).HasDefaultValue("");
            entity.Property(e => e.ArticleType).HasMaxLength(32).IsRequired().HasDefaultValue(ArticleTypes.Article);
            entity.Property(e => e.ParentArticleId).HasDefaultValue(0L);
            entity.Property(e => e.UserId).HasDefaultValue(0L);
            entity.Property(e => e.TopicId).HasDefaultValue(0L);
            entity.Property(e => e.SortNumber).HasDefaultValue(0);
            entity.Property(e => e.IsHidden).IsRequired().HasDefaultValue(0);

            entity.HasIndex(e => new { e.TopicId, e.SortNumber }).HasDatabaseName("IX_Article_TopicId_SortNumber");
            entity.HasIndex(e => e.ParentArticleId).HasDatabaseName("IX_Article_ParentArticleId");
        });
    }
    private void ConfigureTopic(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<Topic>(modelBuilder, [BaseProperty.CreateTime, BaseProperty.UpdateTime, BaseProperty.IsDelete]);

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("Topic");
            entity.Property(e => e.Title).HasMaxLength(256).IsRequired().HasDefaultValue("");
            entity.Property(e => e.Abstract).HasMaxLength(1024).IsRequired().HasDefaultValue("");
            //entity.Property(e => e.Content).HasDefaultValue("");
            entity.Property(e => e.Category).HasMaxLength(256).IsRequired().HasDefaultValue("");
            entity.Property(e => e.TopicImage).HasMaxLength(2048).IsRequired().HasDefaultValue("");
            entity.Property(e => e.UserId).HasDefaultValue(0L);
            entity.Property(e => e.SortNumber).HasDefaultValue(0);
            entity.Property(e => e.IsHidden).IsRequired().HasDefaultValue(0);
        });
    }
    private void ConfigureTag(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<Tag>(modelBuilder, []);

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.Property(e => e.TagName).HasMaxLength(64).IsRequired();
            entity.Property(e => e.TagGroup).HasMaxLength(128);
            entity.Property(e => e.TagColour).HasMaxLength(128);
        });
    }
    private void ConfigureArticleTag(ModelBuilder modelBuilder)
    {
        EFCoreUtil.ConfigBaseEntity<ArticleTag>(modelBuilder, []);

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.ToTable("ArticleTag");
        });
    }
}