using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public class TagRedisRepository : RedisRepository<Tag>, ITagRedisRepository
{
    public TagRedisRepository(IConnectionMultiplexer redis, ILogger<TagRedisRepository> logger)
        : base(redis, logger, hasTime: false)
    {
    }
    public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    {
        names = names.Select(n => n.ToLower().Trim())
                    .Distinct()
                    .Where(n => !string.IsNullOrEmpty(n.Trim()))
                    .ToList();

        var existingTags = await GetAllAsync();

        var existingNames = existingTags.Select(t => t.TagName.ToLower().Trim()).ToList();
        var missingNames = names.Except(existingNames).ToList();

        if (missingNames.Any())
        {
            foreach (var name in missingNames)
            {
                var id = await CreateAsync(new Tag { TagName = name });
                existingTags.Add(new Tag { Id = id, TagName = name });
            }
        }

        return existingTags;
    }
}
