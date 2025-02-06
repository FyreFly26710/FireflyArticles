using Azure.Core;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Interfaces;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Identity.API.Controllers;
[ApiController]
[Route("/user")]
public class UserController : ControllerBase
{
    private IUserService _userService;
    private ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ApiResponse<long>> UserRegister([FromBody] UserRegisterRequest userRegisterRequest)
    {
        // Check for null request
        if (userRegisterRequest == null)
        {
            return ResultUtils.Error<long>(ErrorCode.PARAMS_ERROR);
        }

        string userAccount = userRegisterRequest.UserAccount;
        string userPassword = userRegisterRequest.UserPassword;
        string checkPassword = userRegisterRequest.confirmPassword;

        // Validate input
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(checkPassword))
        {
            return ResultUtils.Error<long>(ErrorCode.PARAMS_ERROR, "User account, password, and confirm password cannot be blank.");
        }

        // Call the user service to register
        long result = await _userService.UserRegister(userAccount, userPassword, checkPassword);

        return ResultUtils.Success(result);
    }

    [HttpPost("login")]
    public async Task<ApiResponse<LoginUserResponse>> UserLogin([FromBody] UserRegisterRequest userRegisterRequest)
    {
        // Check for null request
        if (userRegisterRequest == null)
        {
            return ResultUtils.Error<LoginUserResponse>(ErrorCode.PARAMS_ERROR);
        }
        string userAccount = userRegisterRequest.UserAccount;
        string userPassword = userRegisterRequest.UserPassword;
        // Validate input
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
        {
            return ResultUtils.Error<LoginUserResponse>(ErrorCode.PARAMS_ERROR, "User account and password cannot be blank.");
        }
        // Call the user service to login
        LoginUserResponse result = await _userService.UserLogin(userAccount, userPassword, Request);
        return ResultUtils.Success(result);
    }

    [HttpPost("logout")]
    public async Task<ApiResponse<bool>> UserLogout()
    {
        bool result = await _userService.UserLogout(Request);
        return ResultUtils.Success(result);
    }

    [HttpPost("getLoginUser")]
    public ApiResponse<LoginUserResponse> GetLoginUser()
    {
        var user = _userService.GetLoginUser(Request);
        var loginUserResponse = _userService.GetLoginUserResponse(user);
        return ResultUtils.Success(loginUserResponse);
    }

    //region DB Operation (Admin)
    [HttpPost("delete")]
    public async Task<ApiResponse<bool>> DeleteUser([FromBody] long id)
    {
        if (id <= 0)
        {
            return ResultUtils.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid user id");
        }
        await _userService.DeleteAsync(id);
        return ResultUtils.Success(true);
    }


    //endregion
}