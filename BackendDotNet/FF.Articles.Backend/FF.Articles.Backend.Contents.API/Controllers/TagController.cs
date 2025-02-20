using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Tags;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers;

[ApiController]
[Route("api/contents/tags")]
public class TagController(ILogger<ArticleController> _logger, IMapper _mapper,
    ITagService _tagService)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<TagDto>> GetById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<TagDto>(ErrorCode.PARAMS_ERROR, "Invalid tag id");
        var tag = await _tagService.GetByIdAsync(id);
        if (tag == null)
            return ResultUtil.Error<TagDto>(ErrorCode.NOT_FOUND_ERROR, "Tag not found");
        var tagResponse = _mapper.Map<TagDto>(tag);
        return ResultUtil.Success(tagResponse);
    }
    [HttpGet]
    public async Task<ApiResponse<List<TagDto>>> GetAll()
    {
        var tags = await _tagService.GetAllAsync();
        var tagResponse = _mapper.Map<List<TagDto>>(tags);
        return ResultUtil.Success(tagResponse);
    }
    [HttpPut]
    public async Task<ApiResponse<int>> AddByRequest([FromBody] TagAddRequest tagAddRequest)
    {
        if (tagAddRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);
        var tag = new Tag { TagName = tagAddRequest.TagName };
        int tagId = await _tagService.CreateAsync(tag);
        return ResultUtil.Success(tagId);
    }
    [HttpPost]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] TagEditRequest tagEditRequest)
    {
        if (tagEditRequest == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR);
        var tag = await _tagService.GetByIdAsTrackingAsync(tagEditRequest.TagId);
        if (tag == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Tag not found");
        if (!string.IsNullOrWhiteSpace(tagEditRequest.TagName))
        {
            tag.TagName = tagEditRequest.TagName;
            await _tagService.UpdateAsync(tag);
        }
        return ResultUtil.Success(true);
    }
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR);
        await _tagService.HardDeleteAsync(id);
        return ResultUtil.Success(true);
    }
}
