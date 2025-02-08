using AutoMapper;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;

namespace FF.Articles.Backend.Identity.API.MappingProfiles;
public class ArticleMappingProfile : Profile
{
    public ArticleMappingProfile()
    {
        CreateMap<ArticleAddRequest, Article>();
    }
}