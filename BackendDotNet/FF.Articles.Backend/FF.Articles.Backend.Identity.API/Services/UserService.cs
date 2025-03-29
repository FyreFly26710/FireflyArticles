using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.MapperExtensions.Users;
using FF.Articles.Backend.Identity.API.Models.Dtos;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FF.Articles.Backend.Identity.API.Services;

public class UserService(IUserRepository _userRepository, ILogger<UserService> _logger)
    : BaseService<User, IdentityDbContext>(_userRepository, _logger), IUserService
{
    private const string SALT = "Firefly";

    public async Task<long> UserRegister(string userAccount, string userPassword, string checkPassword)
    {
        validateInputs(userAccount, userPassword, checkPassword);

        var exists = await _userRepository.GetUserByAccount(userAccount);
        if (exists != null)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Account already exists");
        }

        string encryptedPassword = computeMD5Hash(SALT + userPassword);

        var user = new User
        {
            UserName = "Guest",
            UserAccount = userAccount,
            UserPassword = encryptedPassword
        };

        long userId = await _userRepository.CreateAsync(user);
        return userId;

    }

    public async Task<LoginUserDto> UserLogin(string userAccount, string userPassword, HttpRequest request)
    {
        validateInputs(userAccount, userPassword);

        string encryptPassword = computeMD5Hash(SALT + userPassword);

        // Query the user from the database
        var user = await _userRepository.GetUserByAccount(userAccount);

        if (user == null || user.UserPassword != encryptPassword)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "userAccount cannot match userPassword");
        }


        await SignInUser(user, request.HttpContext);
        return user.ToLoginUserDto();
    }

    public async Task<User> GetLoginUser(HttpRequest request)
    {
        //User? user = request.HttpContext.Session.GetObject<User>(UserConstant.USER_LOGIN_STATE);
        var user = UserUtil.GetUserFromHttpRequest(request);
        if (user == null)
        {
            throw new ApiException(ErrorCode.NOT_LOGIN_ERROR);
        }
        User? userEntity = await _userRepository.GetByIdAsync(user.UserId);

        if (userEntity == null)
        {
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "User not found");
        }
        return userEntity;
    }

    public async Task<bool> IsAdmin(HttpRequest request) => IsAdmin(await GetLoginUser(request));

    public bool IsAdmin(User user) => user != null && user.UserRole == UserConstant.ADMIN_ROLE;
    private string computeMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
    private void validateInputs(string userAccount, string userPassword, string confirmPassword)
    {
        validateInputs(userAccount, userPassword);
        if (string.IsNullOrWhiteSpace(confirmPassword))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Missing params");
        if (userPassword != confirmPassword)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Passwords do not match");
    }
    private void validateInputs(string userAccount, string userPassword)
    {
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Missing params");
        if (userAccount.Length < 4)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Account length less than 4");
        if (userPassword.Length < 8)
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Password length less than 8");
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            user = new User() { Id = -1, UserAccount = email, UserRole = "user", UserName = "Guest" };
        }
        return user;
    }

    public async Task SignInUser(User user, HttpContext httpContext)
    {
        // hide password from user object
        user.UserPassword = string.Empty;
        var userDto = new UserApiDto()
        {
            UserId = user.Id,
            CreateTime = user.CreateTime,
            UserAccount = user.UserAccount,
            UserEmail = user.UserEmail,
            UserName = user.UserName,
            UserAvatar = user.UserAvatar,
            UserProfile = user.UserProfile,
            UserRole = user.UserRole
        };
        string userJson = JsonSerializer.Serialize(userDto);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? "Guest"),
            new Claim(ClaimTypes.Role, user.UserRole),
            new Claim("user", userJson),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });
    }
    public async Task SignOutUser(HttpRequest httpRequest)
        => await httpRequest.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

}
