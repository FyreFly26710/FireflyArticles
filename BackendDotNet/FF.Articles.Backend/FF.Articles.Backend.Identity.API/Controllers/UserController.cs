using Azure.Core;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using FF.Articles.Backend.Identity.API.Utils;
using FF.Articles.Backend.Common.Requests;
using FF.Articles.Backend.Identity.API.Services;
using FF.Articles.Backend.Common.Constants;

namespace FF.Articles.Backend.Identity.API.Controllers;
[ApiController]
[Route("api/identity/user")]
public class UserController(IUserService _userService, ILogger<UserController> _logger, IMapper _mapper)
    : ControllerBase
{

    [HttpPost("register")]
    public async Task<ApiResponse<long>> UserRegister([FromBody] UserRegisterRequest userRegisterRequest)
    {
        if (userRegisterRequest == null)
            return ResultUtil.Error<long>(ErrorCode.PARAMS_ERROR);

        string userAccount = userRegisterRequest.UserAccount;
        string userPassword = userRegisterRequest.UserPassword;
        string confirmPassword = userRegisterRequest.confirmPassword;
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            return ResultUtil.Error<long>(ErrorCode.PARAMS_ERROR, "User account, password, and confirm password cannot be blank.");

        long result = await _userService.UserRegister(userAccount, userPassword, confirmPassword);

        return ResultUtil.Success(result);
    }

    [HttpPost("login")]
    public async Task<ApiResponse<LoginUserResponse>> UserLogin([FromBody] UserLoginRequest userLoginRequest)
    {
        if (userLoginRequest == null)
            return ResultUtil.Error<LoginUserResponse>(ErrorCode.PARAMS_ERROR);

        string userAccount = userLoginRequest.UserAccount;
        string userPassword = userLoginRequest.UserPassword;
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
            return ResultUtil.Error<LoginUserResponse>(ErrorCode.PARAMS_ERROR, "User account and password cannot be blank.");

        LoginUserResponse result = await _userService.UserLogin(userAccount, userPassword, Request);
        return ResultUtil.Success(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ApiResponse<bool>> UserLogout()
    {
        await IdentityUtils.SignOutUser(Request);
        return ResultUtil.Success(true);
    }

    [HttpPost("getLoginUser")]
    [Authorize]
    public ApiResponse<LoginUserResponse> GetLoginUser()
    {
        var user = _userService.GetLoginUser(Request);
        var loginUserResponse = _mapper.Map<LoginUserResponse>(user);
        return ResultUtil.Success(loginUserResponse);
    }

}