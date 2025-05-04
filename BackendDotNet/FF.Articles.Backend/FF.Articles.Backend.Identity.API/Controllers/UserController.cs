using FF.Articles.Backend.Common.ApiDtos;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.MapperExtensions.Users;
using FF.Articles.Backend.Identity.API.Models.Dtos;
using FF.Articles.Backend.Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        if (id == 0) throw new ApiException(ErrorCode.PARAMS_ERROR, "Invalid user id");

        await _userService.DeleteAsync(id);
        return ResultUtil.Success(true);
    }

    // Disable edit user endpoint, edit in database directly
    // [HttpPost]
    // [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    // public async Task<ApiResponse<bool>> UpdateByRequest([FromBody] UserUpdateRequest request)
    // {
    //     if (request == null)
    //         return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid user");

    //     User? user = await _userService.GetByIdAsTrackingAsync(request.Id);
    //     if (user == null)
    //         return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "User not found");

    //     _mapper.Map(request, user);
    //     await _userService.UpdateAsync(user);
    //     return ResultUtil.Success(true);
    // }

}
