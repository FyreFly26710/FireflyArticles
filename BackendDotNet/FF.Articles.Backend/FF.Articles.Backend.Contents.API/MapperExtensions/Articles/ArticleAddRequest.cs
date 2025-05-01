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
            cfg.CreateMap<ArticleEditRequest, ArticleAddRequest>();
        });
        _mapper = config.CreateMapper();
    }
    public static Article ToEntity(this ArticleAddRequest articleAddRequest, long? userId = null)
    {
        var article = _mapper.Map<Article>(articleAddRequest);
        if (userId != null)
        {
            article.UserId = userId.Value;
        }
        return article;
    }
    public static ArticleAddRequest ToAddRequest(this ArticleEditRequest articleAddRequest)
    {
        var article = _mapper.Map<ArticleAddRequest>(articleAddRequest);
        article.Id = articleAddRequest.ArticleId;

        return article;
    }
}