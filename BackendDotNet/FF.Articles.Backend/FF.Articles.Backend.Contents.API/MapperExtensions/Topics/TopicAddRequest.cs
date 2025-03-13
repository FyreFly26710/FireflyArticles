using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.MapperExtensions.Topics;
public static class TopicAddRequestExtensions
{
    private static readonly IMapper _mapper;
    static TopicAddRequestExtensions()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TopicAddRequest, Topic>();
        });
        _mapper = config.CreateMapper();
    }
    public static Topic ToEntity(this TopicAddRequest topicAddRequest)
    {
        var topic = _mapper.Map<Topic>(topicAddRequest);
        return topic;
    }
}