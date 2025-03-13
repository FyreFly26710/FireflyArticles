using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Models.Dtos;
using FF.Articles.Backend.Identity.API.Models.Entities;

namespace FF.Articles.Backend.Identity.API.Services;

public interface IUserService : IBaseService<User, IdentityDbContext>
{
    Task<int> UserRegister(string userAccount, string userPassword, string checkPassword);

    Task<LoginUserDto> UserLogin(string userAccount, string userPassword, HttpRequest request);

    User GetLoginUser(HttpRequest request);
    Task<User> GetUserByEmail(string email);

    bool IsAdmin(HttpRequest request);

    bool IsAdmin(User user);

    //Task<bool> UserLogout(HttpRequest request);

    //LoginUserResponse GetLoginUserResponse(User user);

    //UserResponse GetUserResponse(User user);

    //List<UserResponse> GetUserResponse(List<User> userList);

}