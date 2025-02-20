using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.Services.Interfaces;
public interface ITopicService : IBaseService<Topic, ContentsDbContext>
{
    Task<TopicDto> GetTopicDto(Topic topic, bool includeUser = true, bool IncludeArticles = true);
    Task<bool> EditArticleByRequest(TopicEditRequest topicEditRequest);
}
