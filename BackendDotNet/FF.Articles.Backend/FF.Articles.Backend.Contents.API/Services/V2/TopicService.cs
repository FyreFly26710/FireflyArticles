using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.Services.V2;

public class TopicService : RedisService<Topic>, ITopicService
{
    public TopicService(ITopicRedisRepository topicRedisRepository, ILogger<TopicService> logger)
        : base(topicRedisRepository, logger)
    {
    }

    public Task<bool> EditTopicByRequest(TopicEditRequest topicEditRequest)
    {
        throw new NotImplementedException();
    }

    public Task<Topic?> GetTopicByTitle(string title)
    {
        throw new NotImplementedException();
    }

    public Task<TopicDto> GetTopicDto(Topic topic)
    {
        throw new NotImplementedException();
    }

    public Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest)
    {
        throw new NotImplementedException();
    }
}
