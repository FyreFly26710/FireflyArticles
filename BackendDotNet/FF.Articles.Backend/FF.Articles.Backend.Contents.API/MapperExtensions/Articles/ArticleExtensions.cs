using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.MapperExtensions.Articles;
public static class ArticleExtensions
{
    private static readonly IMapper _mapper;
    static ArticleExtensions()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Article, ArticleDto>().ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.Id));
        });
        _mapper = config.CreateMapper();
    }
    public static ArticleDto ToDto(this Article article)
    {
        var articleDto = _mapper.Map<ArticleDto>(article);
        return articleDto;
    }
}