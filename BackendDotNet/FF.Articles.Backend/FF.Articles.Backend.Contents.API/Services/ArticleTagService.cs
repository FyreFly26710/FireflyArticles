using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

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
    public List<String> GetArticleTags(int articleId)
    {
        List<ArticleTag> articleTags = [.. _articleTagRepository.GetQueryable().Where(at => at.ArticleId == articleId)];
        List<String> tagNames = [.. articleTags.Select(at => _tagRepository.GetById(at.TagId)?.TagName ?? "")];

        return tagNames;

    }
    public Dictionary<int, List<String>> GetArticleTags(List<int> articleIds)
    {
        var articleTags = _articleTagRepository.GetQueryable()
            .Where(at => articleIds.Contains(at.ArticleId))
            .ToList();

        var tagIds = articleTags.Select(at => at.TagId).Distinct().ToList();
        var tags = _tagRepository.GetQueryable()
            .Where(t => tagIds.Contains(t.Id))
            .ToDictionary(t => t.Id, t => t.TagName ?? "");

        return articleTags
            .GroupBy(at => at.ArticleId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(at => tags.GetValueOrDefault(at.TagId, "")).ToList()
            );
    }
}