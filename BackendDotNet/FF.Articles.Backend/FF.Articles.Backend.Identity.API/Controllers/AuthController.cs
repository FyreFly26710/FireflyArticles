using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using FF.Articles.Backend.Identity.API.Utils;
using FF.Articles.Backend.Identity.API.Services;

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
    IMapper _mapper, 
    IConfiguration _config)
    : ControllerBase
{

    [HttpPost("register")]
    public async Task<ApiResponse<int>> Register([FromBody] UserRegisterRequest userRegisterRequest)
    {
        if (userRegisterRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);

        string userAccount = userRegisterRequest.UserAccount;
        string userPassword = userRegisterRequest.UserPassword;
        string confirmPassword = userRegisterRequest.confirmPassword;
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR, "User account, password, and confirm password cannot be blank.");

        int result = await _userService.UserRegister(userAccount, userPassword, confirmPassword);

        return ResultUtil.Success(result);
    }
    [HttpPost("login")]
    public async Task<ApiResponse<LoginUserDto>> Login([FromBody] UserLoginRequest userLoginRequest)
    {
        if (userLoginRequest == null)
            return ResultUtil.Error<LoginUserDto>(ErrorCode.PARAMS_ERROR);

        string userAccount = userLoginRequest.UserAccount;
        string userPassword = userLoginRequest.UserPassword;
        if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
            return ResultUtil.Error<LoginUserDto>(ErrorCode.PARAMS_ERROR, "User account and password cannot be blank.");

        LoginUserDto result = await _userService.UserLogin(userAccount, userPassword, Request);
        return ResultUtil.Success(result);
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<ApiResponse<bool>> Logout()
    {
        await IdentityUtils.SignOutUser(Request);
        return ResultUtil.Success(true);
    }
    [HttpPost("getLoginUser")]
    [Authorize]
    public ApiResponse<LoginUserDto> GetLoginUser()
    {
        var user = _userService.GetLoginUser(Request);
        var loginUserResponse = _mapper.Map<LoginUserDto>(user);
        return ResultUtil.Success(loginUserResponse);
    }
    [HttpGet("signin-google")]
    public async Task<IActionResult> SignInGoogle([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is missing.");
        }

        var tokenResponse = await _oAuthService.GetGmailToken(code);

        var userInfo = await _oAuthService.GetUserInfoFromGmailToken(tokenResponse.AccessToken);

        var user = await _userService.GetUserByEmail(userInfo.Email);

        await IdentityUtils.SignIn(user, HttpContext);

        // Redirect to home
        string? homePage = _config["URL:ClientApp"];
        return Redirect(homePage??"/");
    }



}