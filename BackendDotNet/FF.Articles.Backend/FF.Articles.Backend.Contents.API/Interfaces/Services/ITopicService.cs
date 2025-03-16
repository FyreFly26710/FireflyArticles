using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.Interfaces.Services;
public interface ITopicService : IBaseService<Topic, ContentsDbContext>
{
    Task<TopicDto> GetTopicDto(Topic topic);
    Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest);
    Task<bool> EditTopicByRequest(TopicEditRequest topicEditRequest);
}
