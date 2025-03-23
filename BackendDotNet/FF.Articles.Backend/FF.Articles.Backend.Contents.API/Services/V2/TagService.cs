using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Entities;
namespace FF.Articles.Backend.Contents.API.Services.V2;

public class TagService : RedisService<Tag>, ITagService
{
    public TagService(ITagRedisRepository tagRedisRepository, ILogger<TagService> logger)
        : base(tagRedisRepository, logger)
    {
    }
}
