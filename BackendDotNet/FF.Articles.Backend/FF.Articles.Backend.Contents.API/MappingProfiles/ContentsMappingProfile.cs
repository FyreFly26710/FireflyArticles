using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Tags;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.MappingProfiles;
public class ContentsMappingProfile : Profile
{
    public ContentsMappingProfile()
    {
        CreateMap<ArticleAddRequest, Article>();
        CreateMap<Article, ArticleDto>().ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.Id));

        CreateMap<TopicAddRequest, Topic>();
        CreateMap<Topic, TopicDto>().ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.Id));

        CreateMap<TagEditRequest, Tag>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TagId));
        CreateMap<Tag, TagDto>().ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id));
        //CreateMap<ArticlePageRequest, TopicPageRequest>();
        CreateMap<ArticleDto, ArticleMiniDto>().ForMember(dest=>dest.SubArticles, opt=>opt.Ignore());
    }
}