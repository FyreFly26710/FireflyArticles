using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Entities;
namespace FF.Articles.Backend.Contents.API.Services.V2;

public class TagService : RedisService<Tag>, ITagService
{
    private readonly ITagRedisRepository _tagRedisRepository;
    private readonly IArticleTagRedisRepository _articleTagRedisRepository;
    public TagService(ITagRedisRepository tagRedisRepository, IArticleTagRedisRepository articleTagRedisRepository, ILogger<TagService> logger)
        : base(tagRedisRepository, logger)
    {
        _tagRedisRepository = tagRedisRepository;
        _articleTagRedisRepository = articleTagRedisRepository;
    }

    public override async Task<bool> DeleteAsync(long id)
    {
        if (!await _tagRedisRepository.ExistsAsync(id))
            return true;
        await _articleTagRedisRepository.DeleteByTagId(id);
        await _tagRedisRepository.DeleteAsync(id);

        return true;
    }
    public async Task<Tag?> GetTagByNameAsync(string tagName)
    {
        var tags = await _tagRedisRepository.GetAllAsync();
        return tags.FirstOrDefault(t => t.TagName == tagName);
    }
}
