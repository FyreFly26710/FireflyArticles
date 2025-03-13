using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.MapperExtensions.Topics;
public static class TopicExtensions
{
    private static readonly IMapper _mapper;
    static TopicExtensions()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Topic, TopicDto>().ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.Id));
        });
        _mapper = config.CreateMapper();
    }
    public static TopicDto ToDto(this Topic topic)
    {
        var topicDto = _mapper.Map<TopicDto>(topic);
        return topicDto;
    }
}