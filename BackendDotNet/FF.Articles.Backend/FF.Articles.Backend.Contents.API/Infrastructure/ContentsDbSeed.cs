using System;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Infrastructure.Migrations;

public static class ContentsDbSeed
{
    public static async Task InitializeDatabase(WebApplicationBuilder builder)
    {
        // Apply pending migrations
        bool hasPendingMigrations = false;
        using (var scope = builder.Services.BuildServiceProvider().CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ContentsDbContext>();

            hasPendingMigrations = dbContext.Database.GetPendingMigrations().Any();

            dbContext.Database.Migrate();
        }

        // If there are pending migrations, seed the database
        if (hasPendingMigrations)
        {
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var articleRedisRepository = scope.ServiceProvider.GetRequiredService<IArticleRedisRepository>();
                var articleTagRedisRepository = scope.ServiceProvider.GetRequiredService<IArticleTagRedisRepository>();
                var topicRedisRepository = scope.ServiceProvider.GetRequiredService<ITopicRedisRepository>();
                var tagRedisRepository = scope.ServiceProvider.GetRequiredService<ITagRedisRepository>();

                await tagRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetTags());
                await topicRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetTopics());
                await articleRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetArticles());
                await articleTagRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetArticleTags());
            }
        }
    }

}
