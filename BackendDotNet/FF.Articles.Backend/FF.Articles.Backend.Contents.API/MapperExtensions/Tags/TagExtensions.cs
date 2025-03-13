using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.MapperExtensions.Tags;
public static class TagExtensions
{
    private static readonly IMapper _mapper;
    static TagExtensions()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Tag, TagDto>().ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id));
        });
        _mapper = config.CreateMapper();
    }
    public static TagDto ToDto(this Tag tag)
    {
        var tagDto = _mapper.Map<TagDto>(tag);
        return tagDto;
    }
}