using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public class ArticleTagRedisRepository : RedisRepository<ArticleTag>, IArticleTagRedisRepository
{
    public ArticleTagRedisRepository(IConnectionMultiplexer redis, ILogger<ArticleTagRedisRepository> logger)
     : base(redis, logger, hasTime: false)
    {
    }

    //public async Task<List<ArticleTag>> GetByArticleId(long articleId)
    //{
    //    var ids = await GetAllAsync();
    //    return ids.Where(at => at.ArticleId == articleId).ToList();
    //}

    //public async Task<Dictionary<long, List<ArticleTag>>> GetArticleTagGroupsByArticleIds(List<long> articleIds)
    //{
    //    var articleTags = await GetAllAsync();
    //    var articleTagGroups = articleTags
    //        .Where(at => articleIds.Contains(at.ArticleId))
    //        .GroupBy(at => at.ArticleId)
    //        .ToDictionary(g => g.Key, g => g.ToList());
    //    return articleTagGroups;
    //}
    //public async Task<List<ArticleTag>> GetByArticleIds(List<long> articleIds)
    //{
    //    var articleTags = await GetAllAsync();
    //    return articleTags.Where(at => articleIds.Contains(at.ArticleId)).ToList();
    //}
    //public async Task<List<ArticleTag>> GetByTagId(long tagId)
    //{
    //    var articleTags = await GetAllAsync();
    //    return articleTags.Where(at => at.TagId == tagId).ToList();
    //}
    //public async Task<List<ArticleTag>> GetByTagIds(List<long> tagIds)
    //{
    //    var articleTags = await GetAllAsync();
    //    return articleTags.Where(at => tagIds.Contains(at.TagId)).ToList();
    //}

    //public async Task<bool> EditArticleTags(long articleId, List<long> tagIds)
    //{
    //    List<ArticleTag> currentArticleTags = await GetByArticleId(articleId);
    //    foreach (var id in tagIds)
    //    {
    //        if (!currentArticleTags.Any(at => at.TagId == id))
    //            await base.CreateAsync(new ArticleTag { ArticleId = articleId, TagId = id });

    //    }

    //    foreach (var at in currentArticleTags)
    //    {
    //        if (!tagIds.Contains(at.TagId))
    //            await base.DeleteAsync(at.Id);
    //    }
    //    return true;
    //}
    //public async Task<bool> DeleteByArticleId(long articleId)
    //{
    //    var articleTags = await GetByArticleId(articleId);
    //    await base.DeleteBatchAsync(articleTags.Select(at => at.Id).ToList());
    //    return true;
    //}

    //public async Task<bool> DeleteByTagId(long tagId)
    //{
    //    List<ArticleTag> articleTags = await GetByTagId(tagId);
    //    await base.DeleteBatchAsync(articleTags.Select(at => at.Id).ToList());
    //    return true;
    //}
    public Task<bool> DeleteByArticleId(long articleId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByTagId(long tagId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EditArticleTags(long articleId, List<long> tagIds)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<long, List<ArticleTag>>> GetArticleTagGroupsByArticleIds(List<long> articleIds)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArticleTag>> GetByArticleId(long articleId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArticleTag>> GetByArticleIds(List<long> articleIds)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArticleTag>> GetByTagId(long tagId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArticleTag>> GetByTagIds(List<long> tagIds)
    {
        throw new NotImplementedException();
    }
}
