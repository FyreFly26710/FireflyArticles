using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Services;
public class ArticleTagService(
    IArticleTagRepository _articleTagRepository,
    ITagRepository _tagRepository,
    ILogger<ArticleTagService> _logger)
: BaseService<ArticleTag, ContentsDbContext>(_articleTagRepository, _logger), IArticleTagService
{
    public async Task<bool> EditArticleTags(int articleId, List<int> tagIds)
    {
        List<ArticleTag> currentArticleTags = _articleTagRepository.GetQueryable().Where(at => at.ArticleId == articleId).ToList();
        List<int> existingTags = _tagRepository.GetQueryable().Select(t => t.Id).ToList();

        foreach (var id in tagIds)
        {
            if (existingTags.Contains(id) && !currentArticleTags.Any(at => at.TagId == id))
                await _articleTagRepository.CreateAsync(new ArticleTag { ArticleId = articleId, TagId = id });
        }

        foreach (var at in currentArticleTags)
        {
            if (!tagIds.Contains(at.TagId))
                await _articleTagRepository.HardDeleteAsync(at.Id);
        }

        return true;
    }
    public async Task<bool> RemoveArticleTags(int articleId)
    {
        List<ArticleTag> articleTags = _articleTagRepository.GetQueryable().Where(at => at.ArticleId == articleId).ToList();
        foreach (var at in articleTags)
        {
            await _articleTagRepository.HardDeleteAsync(at.Id);
        }
        return true;
    }
    public override async Task<bool> DeleteAsync(int id)
    {
        List<ArticleTag> articleTags = _articleTagRepository.GetQueryable().Where(at => at.TagId == id).ToList();
        foreach (var at in articleTags)
        {
            await _articleTagRepository.HardDeleteAsync(at.Id);
        }
        return true;
    }
    public async Task<List<String>> GetArticleTags(int articleId)
    {
        var articleTags = await _articleTagRepository.GetQueryable()
            .Where(at => at.ArticleId == articleId)
            .ToListAsync();

        if (!articleTags.Any())
            return new List<string>();

        var tagIds = articleTags.Select(at => at.TagId).ToList();

        var tags = await _tagRepository.GetByIdsAsync(tagIds);

        return tags
            .Where(t => t != null && !string.IsNullOrEmpty(t.TagName))
            .Select(t => t.TagName!)
            .ToList();
    }
    public async Task<Dictionary<int, List<String>>> GetArticleTags(List<int> articleIds)
    {
        var articleTags = await _articleTagRepository.GetQueryable()
            .Where(at => articleIds.Contains(at.ArticleId))
            .ToListAsync();

        var tagIds = articleTags.Select(at => at.TagId).Distinct().ToList();
        var tags = await _tagRepository.GetQueryable()
            .Where(t => tagIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.TagName ?? "");

        return articleTags
            .GroupBy(at => at.ArticleId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(at => tags.GetValueOrDefault(at.TagId, "")).ToList()
            );
    }
}