using System;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Models.Responses;
using FF.Articles.Backend.Identity.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Identity.API.Services;

public class UserService(IdentityDbContext _context, ILogger<UserService> _logger, IMapper _mapper)
    : BaseService<User, IdentityDbContext>(_context, _logger), IUserService
{
    private const string SALT = "Firefly";

    public async Task<int> UserRegister(string userAccount, string userPassword, string checkPassword)
    {
        validateAccount(userAccount, userPassword, checkPassword);

        var exists = await this.GetQueryable().AnyAsync(u => u.UserAccount == userAccount);
        //var exists = await _context.Set<User>().AnyAsync(u => u.UserAccount == userAccount);
        if (exists)
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

        int userId = await this.CreateAsync(user);
        return userId;

    }

    public async Task<LoginUserDto> UserLogin(string userAccount, string userPassword, HttpRequest request)
    {
        validateAccount(userAccount, userPassword);

        string encryptPassword = computeMD5Hash(SALT + userPassword);

        // Query the user from the database
        var user = await this.GetQueryable()
            .FirstOrDefaultAsync(u => u.UserAccount == userAccount && u.UserPassword == encryptPassword);

        if (user == null)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "userAccount cannot match userPassword");
        }

        await IdentityUtils.SignIn(user, request.HttpContext);
        return _mapper.Map<LoginUserDto>(user);
    }

    public User GetLoginUser(HttpRequest request)
    {
        //User? user = request.HttpContext.Session.GetObject<User>(UserConstant.USER_LOGIN_STATE);
        var user = UserUtil.GetUserFromHttpRequest(request);
        if (user == null)
        {
            throw new ApiException(ErrorCode.NOT_LOGIN_ERROR);
        }
        // need to get latest state of user??
        // user = _context.Users.Find(user.Id);
        
        return _mapper.Map<User>(user);
    }

    public bool IsAdmin(HttpRequest request) => IsAdmin(GetLoginUser(request));

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
    private void validateAccount(string userAccount, string userPassword, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Missing params");
        }
        if (userAccount.Length < 4)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Account length less than 4");
        }
        if (userPassword.Length < 8 || confirmPassword.Length < 8)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Password length less than 8");
        }
        if (userPassword != confirmPassword)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Passwords do not match");
        }
    }
    private void validateAccount(string userAccount, string userPassword)
    {
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Missing params");
        }
        if (userAccount.Length < 4)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Account length less than 4");
        }
        if (userPassword.Length < 8)
        {
            throw new ApiException(ErrorCode.PARAMS_ERROR, "Password length less than 8");
        }
    }
}
