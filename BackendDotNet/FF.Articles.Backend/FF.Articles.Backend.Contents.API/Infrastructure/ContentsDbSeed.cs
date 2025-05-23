using FF.Articles.Backend.Common.BackgoundJobs;

namespace FF.Articles.Backend.Contents.API.Infrastructure;

public class ContentsDbSeed : IDbSeeder<ContentsDbContext>
{
    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        // var articleRedisRepository = scope.ServiceProvider.GetRequiredService<IArticleRedisRepository>();
        // var articleTagRedisRepository = scope.ServiceProvider.GetRequiredService<IArticleTagRedisRepository>();
        // var topicRedisRepository = scope.ServiceProvider.GetRequiredService<ITopicRedisRepository>();
        // var tagRedisRepository = scope.ServiceProvider.GetRequiredService<ITagRedisRepository>();

        // await tagRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetTags());
        // await topicRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetTopics());
        // await articleRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetArticles());
        // await articleTagRedisRepository.CreateBatchAsync(ContentsDbSeedData.GetArticleTags());
        var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
        var articleTagRepository = scope.ServiceProvider.GetRequiredService<IArticleTagRepository>();
        var tagRepository = scope.ServiceProvider.GetRequiredService<ITagRepository>();
        var topicRepository = scope.ServiceProvider.GetRequiredService<ITopicRepository>();

        await articleRepository.CreateBatchAsync(ContentsDbSeedData.GetArticles());
        await articleTagRepository.CreateBatchAsync(ContentsDbSeedData.GetArticleTags());
        await tagRepository.CreateBatchAsync(ContentsDbSeedData.GetTags());
        await topicRepository.CreateBatchAsync(ContentsDbSeedData.GetTopics());
        await articleRepository.SaveChangesAsync();
    }
}
