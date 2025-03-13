using AutoMapper;
using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Identity.API.Models.Dtos;
using FF.Articles.Backend.Identity.API.Models.Entities;

namespace FF.Articles.Backend.Identity.API.MapperExtensions.Users;
public static class UserExtensions
{
    private static readonly IMapper _mapper;
    static UserExtensions()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>();
            cfg.CreateMap<User, LoginUserDto>();
            cfg.CreateMap<User, UserApiDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
        });
        _mapper = config.CreateMapper();
    }
    public static UserDto ToDto(this User user)
    {
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }
    public static LoginUserDto ToLoginUserDto(this User user)
    {
        var loginUserDto = _mapper.Map<LoginUserDto>(user);
        return loginUserDto;
    }
    public static UserApiDto ToUserApiDto(this User user)
    {
        var userApiDto = _mapper.Map<UserApiDto>(user);
        return userApiDto;
    }
}