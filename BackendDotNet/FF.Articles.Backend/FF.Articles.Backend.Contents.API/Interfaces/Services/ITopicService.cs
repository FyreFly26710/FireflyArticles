﻿using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.Interfaces.Services;
public interface ITopicService : IBaseService<Topic>
{
    Task<TopicDto> GetTopicDto(Topic topic);
    Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest);
    Task<bool> EditTopicByRequest(TopicEditRequest topicEditRequest);
    Task<Topic?> GetTopicByTitle(string title);
}
