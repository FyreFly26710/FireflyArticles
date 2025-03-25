using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public class ArticleTagRedisRepository : RedisRepository<ArticleTag>, IArticleTagRedisRepository
{
    public ArticleTagRedisRepository(IConnectionMultiplexer redis, ILogger<ArticleTagRedisRepository> logger)
     : base(redis, logger, hasTime: false)
    {
    }

    public async Task<List<ArticleTag>> GetByArticleId(int articleId)
    {
        var ids = await GetAllAsync();
        return ids.Where(at => at.ArticleId == articleId).ToList();
    }

    public async Task<Dictionary<int, List<ArticleTag>>> GetArticleTagGroupsByArticleIds(List<int> articleIds)
    {
        var articleTags = await GetAllAsync();
        var articleTagGroups = articleTags
            .Where(at => articleIds.Contains(at.ArticleId))
            .GroupBy(at => at.ArticleId)
            .ToDictionary(g => g.Key, g => g.ToList());
        return articleTagGroups;
    }
    public async Task<List<ArticleTag>> GetByArticleIds(List<int> articleIds)
    {
        var articleTags = await GetAllAsync();
        return articleTags.Where(at => articleIds.Contains(at.ArticleId)).ToList();
    }
    public async Task<List<ArticleTag>> GetByTagId(int tagId)
    {
        var articleTags = await GetAllAsync();
        return articleTags.Where(at => at.TagId == tagId).ToList();
    }
    public async Task<List<ArticleTag>> GetByTagIds(List<int> tagIds)
    {
        var articleTags = await GetAllAsync();
        return articleTags.Where(at => tagIds.Contains(at.TagId)).ToList();
    }

    public async Task<bool> EditArticleTags(int articleId, List<int> tagIds)
    {
        List<ArticleTag> currentArticleTags = await GetByArticleId(articleId);
        foreach (var id in tagIds)
        {
            if (!currentArticleTags.Any(at => at.TagId == id))
                await base.CreateAsync(new ArticleTag { ArticleId = articleId, TagId = id });

        }

        foreach (var at in currentArticleTags)
        {
            if (!tagIds.Contains(at.TagId))
                await base.DeleteAsync(at.Id);
        }
        return true;
    }
    public async Task<bool> DeleteByArticleId(int articleId)
    {
        var articleTags = await GetByArticleId(articleId);
        await base.DeleteBatchAsync(articleTags.Select(at => at.Id).ToList());
        return true;
    }

    public async Task<bool> DeleteByTagId(int tagId)
    {
        List<ArticleTag> articleTags = await GetByTagId(tagId);
        await base.DeleteBatchAsync(articleTags.Select(at => at.Id).ToList());
        return true;
    }

}
