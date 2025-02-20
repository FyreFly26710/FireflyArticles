using AutoMapper;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Models.Responses;

namespace FF.Articles.Backend.Identity.API.MappingProfiles;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, LoginUserDto>();

        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserUpdateRequest, User>();
        CreateMap<User, UserApiDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id)).ReverseMap();
    }
}

