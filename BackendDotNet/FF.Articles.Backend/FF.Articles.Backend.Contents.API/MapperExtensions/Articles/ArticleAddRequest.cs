using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Contents.API.MapperExtensions.Articles;
public static class ArticleAddRequestExtensions
{
    private static readonly IMapper _mapper;
    static ArticleAddRequestExtensions()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ArticleAddRequest, Article>();
        });
        _mapper = config.CreateMapper();
    }
    public static Article ToEntity(this ArticleAddRequest articleAddRequest, int? userId = null)
    {
        var article = _mapper.Map<Article>(articleAddRequest);
        if (userId.HasValue)
        {
            article.UserId = userId.Value;
        }
        return article;
    }
}