using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Models.Entities;
using StackExchange.Redis;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;

public class TopicRedisRepository : RedisRepository<Topic>, ITopicRedisRepository
{
    public TopicRedisRepository(IConnectionMultiplexer redis, ILogger<TopicRedisRepository> logger)
        : base(redis, logger)
    {
    }
}
