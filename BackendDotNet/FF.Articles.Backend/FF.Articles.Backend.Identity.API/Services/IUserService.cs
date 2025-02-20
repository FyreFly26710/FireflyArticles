using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Models.Responses;

namespace FF.Articles.Backend.Identity.API.Services;

public interface IUserService : IBaseService<User, IdentityDbContext>
{
    Task<int> UserRegister(string userAccount, string userPassword, string checkPassword);

    Task<LoginUserDto> UserLogin(string userAccount, string userPassword, HttpRequest request);

    User GetLoginUser(HttpRequest request);

    bool IsAdmin(HttpRequest request);

    bool IsAdmin(User user);

    //Task<bool> UserLogout(HttpRequest request);

    //LoginUserResponse GetLoginUserResponse(User user);

    //UserResponse GetUserResponse(User user);

    //List<UserResponse> GetUserResponse(List<User> userList);

}