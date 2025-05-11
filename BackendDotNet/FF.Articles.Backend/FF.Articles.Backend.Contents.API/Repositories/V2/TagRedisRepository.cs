using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public class TagRedisRepository : RedisRepository<Tag>, ITagRedisRepository
{
    public TagRedisRepository(IConnectionMultiplexer redis, ILogger<TagRedisRepository> logger)
        : base(redis, logger, hasTime: false)
    {
    }

    public Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    {
        throw new NotImplementedException();
    }
    //public async Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names)
    //{
    //    // Normalize input names
    //    names = names.Select(n => n.ToLower().Trim())
    //                .Distinct()
    //                .Where(n => !string.IsNullOrEmpty(n.Trim()))
    //                .ToList();

    //    if (!names.Any())
    //        return new List<Tag>();

    //    var existingTags = await GetAllAsync();

    //    // Create a dictionary for faster lookup
    //    var tagDict = existingTags.ToDictionary(t => t.TagName.ToLower().Trim(), t => t);

    //    // Find missing names that need to be created
    //    var missingNames = names.Where(n => !tagDict.ContainsKey(n)).ToList();

    //    // Create missing tags
    //    foreach (var name in missingNames)
    //    {
    //        var id = await CreateAsync(new Tag { TagName = name });
    //        var newTag = new Tag { Id = id, TagName = name };
    //        tagDict[name] = newTag;
    //    }

    //    // Return only the requested tags in the same order as input names
    //    return names.Select(n => tagDict[n]).ToList();
    //}
}
