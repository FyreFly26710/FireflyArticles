using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class ArticleTagService(ContentsDbContext _context, ILogger<ArticleTagService> _logger, IMapper _mapper)
: CacheService<ArticleTag, ContentsDbContext>(_context, _logger), IArticleTagService
{
    public async Task<bool> EditArticleTags(int articleId, List<int> tagIds)
    {
        List<ArticleTag> currentArticleTags = _context.Set<ArticleTag>().AsQueryable().Where(at => at.ArticleId == articleId).ToList();
        List<int> existingTags = _context.Set<Tag>().AsQueryable().Select(t => t.Id).ToList();

        foreach (var id in tagIds)
        {
            if (existingTags.Contains(id) && !currentArticleTags.Any(at => at.TagId == id))
                await this.CreateAsync(new ArticleTag { ArticleId = articleId, TagId = id });
        }

        foreach (var at in currentArticleTags)
        {
            if (!tagIds.Contains(at.TagId))
                await this.HardDeleteAsync(at.Id);
        }
        
        return true;
    }
    public async Task<bool> RemoveArticleTags(int articleId)
    {
        List<ArticleTag> articleTags = this.GetQueryable().Where(at => at.ArticleId == articleId).ToList();
        foreach (var at in articleTags)
        {
            await this.HardDeleteAsync(at.Id);
        }
        return true;
    }
    public List<String> GetArticleTags(int articleId)
    {
        List<ArticleTag> articleTags = this.GetQueryable().Where(at => at.ArticleId == articleId).ToList();
        List<String> tagNames = new List<String>();
        foreach (var at in articleTags)
        {
            Tag? tag = _context.Set<Tag>().AsQueryable().FirstOrDefault(t => t.Id == at.TagId);
            if (tag != null)
                tagNames.Add(tag.TagName);
        }
        return tagNames;

    }
}