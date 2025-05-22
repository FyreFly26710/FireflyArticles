namespace FF.Articles.Backend.Identity.API.Controllers;
[Route("api/identity/users")]
[ApiController]
public class UserController(IUserService _userService, ILogger<UserController> _logger)
: ControllerBase
{
    //[EnableCors("AllowedBackendUrls")]
    [HttpGet("{id}")]
    public async Task<ApiResponse<UserApiDto>> GetById(long id)
    {
        if (id == 0) throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid user id");
        var user = await _userService.GetByIdAsync(id);
        if (user == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "User not found");
        var userDto = user.ToUserApiDto();
        return ResultUtil.Success(userDto);
    }

    [HttpGet]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<Paged<UserDto>>> GetByPage([FromQuery] PageRequest pageRequest)
    {
        var users = await _userService.GetAllAsync(pageRequest);
        var useDtos = users.Data.Select(user => user.ToDto()).ToList();
        var response = new Paged<UserDto>(users.PageIndex, users.PageSize, users.Counts, useDtos);
        return ResultUtil.Success(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteById(long id)
    {
        if (EnvUtil.IsProduction())
            throw new ApiException(ErrorCode.PARAMS_ERROR, "User delete is disabled in production environment");
        if (id == 0) throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid user id");

        await _userService.DeleteAsync(id);
        return ResultUtil.Success(true);
    }

    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> UpdateByRequest([FromBody] UserUpdateRequest request)
    {
        var user = await _userService.GetByIdAsync(request.Id);
        if (user == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "User not found");
        if (!string.IsNullOrWhiteSpace(request.UserEmail))
            user.UserEmail = request.UserEmail;
        if (!string.IsNullOrWhiteSpace(request.UserName))
            user.UserName = request.UserName;
        if (!string.IsNullOrWhiteSpace(request.UserAvatar))
            user.UserAvatar = request.UserAvatar;
        if (!string.IsNullOrWhiteSpace(request.UserProfile))
            user.UserProfile = request.UserProfile;
        await _userService.UpdateAsync(user);
        return ResultUtil.Success(true);
    }

}
