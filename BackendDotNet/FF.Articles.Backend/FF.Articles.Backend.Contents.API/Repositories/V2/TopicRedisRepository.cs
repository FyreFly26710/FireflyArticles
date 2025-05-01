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
    //public async override Task<bool> DeleteAsync(long id)
    //{
    //    var result = await base.DeleteAsync(id);
    //    await _redis.SetRemoveAsync(RedisIndex.TOPIC_INDEX, id);
    //    return result;
    //}
    //public async override Task<bool> DeleteBatchAsync(List<long> ids)
    //{
    //    var result = await base.DeleteBatchAsync(ids);
    //    await _redis.SetRemoveAsync(RedisIndex.TOPIC_INDEX, ids.Select(id => (RedisValue)id).ToArray());
    //    return result;
    //}

}
