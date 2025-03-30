using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.MapperExtensions.Tags;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers;

public abstract class TagControllerBase : ControllerBase
{
    private readonly ITagService _tagService;
    public TagControllerBase(Func<string, ITagService> tagService, string version)
    {
        _tagService = tagService(version);
    }
    [HttpGet("{id}")]
    public async Task<ApiResponse<TagDto>> GetById(long id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        if (tag == null) throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Tag not found");
        var tagResponse = tag.ToDto();
        return ResultUtil.Success(tagResponse);
    }
    [HttpGet]
    public async Task<ApiResponse<List<TagDto>>> GetAll()
    {
        //var user = UserUtil.GetUserFromHttpRequest(Request);
        var tags = await _tagService.GetAllAsync();
        var tagResponse = tags.Select(t => t.ToDto()).ToList();
        return ResultUtil.Success(tagResponse);
    }
    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<string>> AddByRequest([FromBody] TagAddRequest tagAddRequest)
    {
        var tag = await _tagService.GetTagByNameAsync(tagAddRequest.TagName);
        if (tag != null) throw new ApiException(ErrorCode.PARAMS_ERROR, "Tag already exists");
        var newTag = new Tag { TagName = tagAddRequest.TagName };
        long tagId = await _tagService.CreateAsync(newTag);
        return ResultUtil.Success(tagId.ToString());
    }
    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] TagEditRequest tagEditRequest)
    {
        var tag = await _tagService.GetByIdAsync(tagEditRequest.TagId);
        if (tag == null) throw new ApiException(ErrorCode.PARAMS_ERROR, "Tag not found");
        if (!string.IsNullOrWhiteSpace(tagEditRequest.TagName))
        {
            tag.TagName = tagEditRequest.TagName;
            await _tagService.UpdateAsync(tag);
        }
        return ResultUtil.Success(true);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> DeleteById(long id)
    {
        await _tagService.DeleteAsync(id);
        return ResultUtil.Success(true);
    }
}
