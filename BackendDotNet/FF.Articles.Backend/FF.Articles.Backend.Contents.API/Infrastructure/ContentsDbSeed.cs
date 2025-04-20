using System;
using FF.Articles.Backend.Common.BackgoundJobs;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Infrastructure.Migrations;

public class ContentsDbSeed : IDbSeeder<ContentsDbContext>
{
    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
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
