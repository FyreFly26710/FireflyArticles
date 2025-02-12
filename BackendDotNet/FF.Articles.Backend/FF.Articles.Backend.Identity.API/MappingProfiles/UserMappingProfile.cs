using AutoMapper;
using FF.Articles.Backend.Common.Dtos;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Models.Responses;

namespace FF.Articles.Backend.Identity.API.MappingProfiles;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, LoginUserResponse>();

        CreateMap<User, UserResponse>().ReverseMap();
        CreateMap<UserUpdateRequest, User>();
        CreateMap<User, UserDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id)).ReverseMap();
    }
}

