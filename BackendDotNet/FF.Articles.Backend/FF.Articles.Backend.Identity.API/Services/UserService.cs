using System;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Identity.API.Constants;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Interfaces;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Models.Responses;
using FF.Articles.Backend.Identity.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Identity.API.Services;

    public class UserService(IdentityDbContext _context, ILogger<UserService> _logger, IMapper _mapper)
        : BaseService<User,IdentityDbContext>(_context, _logger), IUserService
    {
        private const string SALT = "Firefly";

        public async Task<long> UserRegister(string userAccount, string userPassword, string checkPassword)
        {
            validateAccount(userAccount, userPassword, checkPassword);
            
            bool exists = _context.Set<User>().Any(u => u.UserAccount == userAccount);
            if (exists)
            {
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Account already exists");
            }

            string encryptedPassword = computeMD5Hash(SALT + userPassword);

            var user = new User
            {
                UserAccount = userAccount,
                UserPassword = encryptedPassword
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Id;

        }

        public async Task<LoginUserResponse> UserLogin(string userAccount, string userPassword, HttpRequest request)
        {
            validateAccount(userAccount, userPassword, null);

            // Hash the password
            string encryptPassword = computeMD5Hash(SALT + userPassword);

            // Query the user from the database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserAccount == userAccount && u.UserPassword == encryptPassword);

            if (user == null)
            {
                throw new ApiException(ErrorCode.PARAMS_ERROR, "userAccount cannot match userPassword");
            }

            await IdentityUtils.SignInUser(user, request.HttpContext);
            return GetLoginUserResponse(user);
        }

        public User GetLoginUser(HttpRequest request)
        {
            //User? user = request.HttpContext.Session.GetObject<User>(UserConstant.USER_LOGIN_STATE);
            User? user = IdentityUtils.GetUserFromHttpRequest(request);
            if (user == null)
            {
                throw new ApiException(ErrorCode.NOT_LOGIN_ERROR);
            }
            // need to get latest state of user??
            // user = _context.Users.Find(user.Id);
            return (User)user;
        }

        public bool IsAdmin(HttpRequest request) => IsAdmin(GetLoginUser(request));

        public bool IsAdmin(User user) => user != null && user.UserRole == UserConstant.ADMIN_ROLE;

        public async Task<bool> UserLogout(HttpRequest request) => await IdentityUtils.SignOutUser(request);
 

        public LoginUserResponse GetLoginUserResponse(User user) => _mapper.Map<LoginUserResponse>(user);
        public UserResponse GetUserResponse(User user) =>  _mapper.Map<UserResponse>(user);
        public List<UserResponse> GetUserResponse(List<User> userList) =>_mapper.Map<List<UserResponse>>(userList);

        private string computeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        private void validateAccount(string userAccount, string userPassword, string? checkPassword)
        {
            if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(checkPassword))
            {
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Missing params");
            }
            if (userAccount.Length < 4)
            {
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Account length less than 4");
            }
            if (userPassword.Length < 8 || (checkPassword!=null && checkPassword.Length < 8))
            {
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Password length less than 8");
            }
            if (checkPassword != null && (checkPassword!=null || userPassword != checkPassword))
            {
                throw new ApiException(ErrorCode.PARAMS_ERROR, "Passwords do not match");
            }
        }
    }
