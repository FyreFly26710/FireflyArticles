using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.MapperExtensions.Users;
using FF.Articles.Backend.Identity.API.Models.Dtos;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Identity.API.Controllers;

/// <summary>
/// Authentication funcaional API
/// </summary>
[ApiController]
[Route("api/identity/auth")]
public class AuthController(
    IUserService _userService,
    IOAuthService _oAuthService,
    ILogger<AuthController> _logger,
    IConfiguration _config)
    : ControllerBase
{

    [HttpPost("register")]
    public async Task<ApiResponse<string>> Register([FromBody] UserRegisterRequest userRegisterRequest)
    {
        if (userRegisterRequest == null) throw new ApiException(ErrorCode.PARAMS_ERROR, "User register request is null");

        string userAccount = userRegisterRequest.UserAccount;
        string userPassword = userRegisterRequest.UserPassword;
        string confirmPassword = userRegisterRequest.confirmPassword;
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "User account, password, and confirm password cannot be blank.");

        long result = await _userService.UserRegister(userAccount, userPassword, confirmPassword);

        return ResultUtil.Success(result.ToString());
    }
    [HttpPost("login")]
    public async Task<ApiResponse<LoginUserDto>> Login([FromBody] UserLoginRequest userLoginRequest)
    {
        if (userLoginRequest == null) throw new ApiException(ErrorCode.PARAMS_ERROR, "User login request is null");

        string userAccount = userLoginRequest.UserAccount;
        string userPassword = userLoginRequest.UserPassword;
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
            throw new ApiException(ErrorCode.PARAMS_ERROR, "User account and password cannot be blank.");

        LoginUserDto result = await _userService.UserLogin(userAccount, userPassword, Request);
        return ResultUtil.Success(result);
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<ApiResponse<bool>> Logout()
    {
        await _userService.SignOutUser(Request);
        return ResultUtil.Success(true);
    }
    [HttpPost("getLoginUser")]
    [Authorize]
    public async Task<ApiResponse<LoginUserDto>> GetLoginUser()
    {
        var user = await _userService.GetLoginUser(Request);
        var loginUserResponse = user.ToLoginUserDto();
        return ResultUtil.Success(loginUserResponse);
    }
    [HttpGet("signin-google")]
    public async Task<IActionResult> SignInGoogle([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code)) throw new ApiException(ErrorCode.PARAMS_ERROR, "Authorization code is missing.");

        var tokenResponse = await _oAuthService.GetGmailToken(code);

        var userInfo = await _oAuthService.GetUserInfoFromGmailToken(tokenResponse.AccessToken);

        var user = await _userService.GetUserByEmail(userInfo.Email);

        await _userService.SignInUser(user, HttpContext);

        // Redirect to home
        string? homePage = _config["Domain:Home"];
        return Redirect(homePage ?? "/");
    }



}