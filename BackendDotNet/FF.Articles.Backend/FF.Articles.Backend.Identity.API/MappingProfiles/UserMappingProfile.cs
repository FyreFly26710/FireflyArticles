using AutoMapper;
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
    }
}