using AutoMapper;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Dtos;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Requests;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Identity.API.Models.Entities;
using FF.Articles.Backend.Identity.API.Models.Requests;
using FF.Articles.Backend.Identity.API.Models.Responses;
using FF.Articles.Backend.Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Identity.API.Controllers;
[Route("api/identity/admin")]
[ApiController]
public class AdminController(IUserService _userService, ILogger<AdminController> _logger, IMapper _mapper)
: ControllerBase
{

    [HttpPost("list")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<PageResponse<UserResponse>>> ListUsers([FromBody] PageRequest pageRequest)
    {
        var users = await _userService.GetAllAsync(pageRequest);
        var useDtos = _mapper.Map<List<UserResponse>>(users.Data);
        var response = new PageResponse<UserResponse>(users.PageIndex, users.PageSize, users.RecordCount, useDtos);
        return ResultUtil.Success(response);
    }


    [HttpPost("delete")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteUser([FromBody] int id)
    {
        if (id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid user id");

        await _userService.DeleteAsync(id);
        return ResultUtil.Success(true);
    }

    [HttpPost("update")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> UpdateUser([FromBody] UserUpdateRequest request)
    {
        if (request == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid user");

        User user = await _userService.GetByIdAsTrackingAsync(request.Id);
        if (user == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "User not found");

        _mapper.Map(request, user);
        await _userService.UpdateAsync(user);
        return ResultUtil.Success(true);
    }

    [EnableCors("AllowedBackendUrls")]
    [HttpGet("get-dto/{id}")]
    public async Task<ApiResponse<UserDto>> GetUserDto(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<UserDto>(ErrorCode.PARAMS_ERROR, "Invalid user id");
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return ResultUtil.Error<UserDto>(ErrorCode.NOT_FOUND_ERROR, "User not found");
        var userDto = _mapper.Map<UserDto>(user);
        return ResultUtil.Success(userDto);
    }
}
