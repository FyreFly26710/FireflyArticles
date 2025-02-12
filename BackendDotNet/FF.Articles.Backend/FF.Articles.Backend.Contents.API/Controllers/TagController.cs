using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Requests;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Tags;
using FF.Articles.Backend.Contents.API.Models.Responses;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers;

[ApiController]
[Route("api/contents/tag")]
public class TagController(ILogger<ArticleController> _logger, IMapper _mapper,
    ITagService _tagService)
    : ControllerBase
{

    [HttpGet("get/{id}")]
    public async Task<ApiResponse<TagResponse>> GetById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<TagResponse>(ErrorCode.PARAMS_ERROR, "Invalid tag id");
        var tag = await _tagService.GetByIdAsync(id);
        if (tag == null)
            return ResultUtil.Error<TagResponse>(ErrorCode.NOT_FOUND_ERROR, "Tag not found");
        var tagResponse = _mapper.Map<TagResponse>(tag);
        return ResultUtil.Success(tagResponse);
    }
    [HttpGet("get/all")]
    public async Task<ApiResponse<List<TagResponse>>> GetAll()
    {
        var tags = await _tagService.GetAllAsync();
        var tagResponse = _mapper.Map<List<TagResponse>>(tags);
        return ResultUtil.Success(tagResponse);
    }

    [HttpPost("add")]
    public async Task<ApiResponse<int>> AddTag([FromBody] TagAddRequest tagAddRequest)
    {
        if (tagAddRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);
        var tag = new Tag { TagName = tagAddRequest.TagName };
        int tagId = await _tagService.CreateAsync(tag);
        return ResultUtil.Success(tagId);
    }
    [HttpPost("edit")]
    public async Task<ApiResponse<int>> EditTag([FromBody] TagEditRequest tagEditRequest)
    {
        if (tagEditRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);
        var tag = await _tagService.GetByIdAsTrackingAsync(tagEditRequest.TagId);
        if (tag == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR, "Tag not found");
        if (!string.IsNullOrWhiteSpace(tagEditRequest.TagName))
        {
            tag.TagName = tagEditRequest.TagName;
            await _tagService.UpdateAsync(tag);
        }
        return ResultUtil.Success(tag.Id);
    }
    [HttpPost("delete")]
    public async Task<ApiResponse<bool>> DeleteTag([FromBody] DeleteByIdRequest deleteByIdRequest)
    {
        if (deleteByIdRequest == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR);
        var tag = await _tagService.GetByIdAsTrackingAsync(deleteByIdRequest.Id);
        if (tag == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Tag not found");
        await _tagService.HardDeleteAsync(tag.Id);
        return ResultUtil.Success(true);
    }
}
