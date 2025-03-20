using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;
public class ArticleTagRepository : BaseRepository<ArticleTag, ContentsDbContext>, IArticleTagRepository
{
    public ArticleTagRepository(ContentsDbContext _context) : base(_context)
    {
    }

    public async Task<List<ArticleTag>> GetByArticleId(int articleId)
    {
        return await base.GetQueryable().Where(at => at.ArticleId == articleId).ToListAsync();
    }
    public async Task<List<ArticleTag>> GetByArticleIds(List<int> articleIds)
    {
        return await base.GetQueryable().Where(at => articleIds.Contains(at.ArticleId)).ToListAsync();
    }
    public async Task<List<ArticleTag>> GetByTagId(int tagId)
    {
        return await base.GetQueryable().Where(at => at.TagId == tagId).ToListAsync();
    }
    public async Task<List<ArticleTag>> GetByTagIds(List<int> tagIds)
    {
        return await base.GetQueryable().Where(at => tagIds.Contains(at.TagId)).ToListAsync();
    }

    public async Task<bool> EditArticleTags(int articleId, List<int> tagIds)
    {
        List<ArticleTag> currentArticleTags = await GetByArticleId(articleId);
        List<int> existingTags = await _context.Set<Tag>().Select(t => t.Id).ToListAsync();
        foreach (var id in tagIds)
        {
            if (existingTags.Contains(id) && !currentArticleTags.Any(at => at.TagId == id))
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

    public async Task<Dictionary<int, List<Tag>>> GetTagGroupsByArticleIds(List<int> articleIds)
    {
        // Get article tags and their corresponding tags using a join
        var query = from at in _context.Set<ArticleTag>()
                    join tag in _context.Set<Tag>()
                    on at.TagId equals tag.Id
                    where articleIds.Contains(at.ArticleId)
                    select new { at.ArticleId, Tag = tag };

        var results = await query.ToListAsync();

        // Group into dictionary
        return results
            .GroupBy(x => x.ArticleId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.Tag).ToList()
            );
    }

}
