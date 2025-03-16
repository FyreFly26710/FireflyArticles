using AutoMapper;
using FF.Articles.Backend.Common.Constants;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.MapperExtensions.Topics;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;
using FF.Articles.Backend.Contents.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/topics")]
public class TopicController(ILogger<TopicController> _logger,
    ITopicService _topicService)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<TopicDto>> GetById(int id, [FromQuery] TopicQueryRequest query)
    {
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null)
            return ResultUtil.Error<TopicDto>(ErrorCode.NOT_FOUND_ERROR, "Topic not found");
        TopicDto topicDto = await _topicService.GetTopicDto(topic, query);
        return ResultUtil.Success(topicDto);
    }
    [HttpGet]
    public async Task<ApiResponse<Paged<TopicDto>>> GetByPage([FromQuery] TopicQueryRequest pageRequest)
    {
        if (pageRequest == null || pageRequest.PageSize > 200)
            return ResultUtil.Error<Paged<TopicDto>>(ErrorCode.PARAMS_ERROR, "Invalid page request");
        pageRequest.SortField ??= "SortNumber";

        Paged<Topic> topics = await _topicService.GetAllAsync(pageRequest);
        Paged<TopicDto> res = new(topics.GetPageInfo());
        foreach (var topic in topics.Data)
        {
            TopicDto topicDto = await _topicService.GetTopicDto(topic, pageRequest);
            res.Data.Add(topicDto);
        }
        return ResultUtil.Success(res);
    }

    [HttpPut]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<int>> AddByRequest([FromBody] TopicAddRequest topicAddRequest)
    {
        string title = topicAddRequest.Title;
        var existTopic = _topicService.GetQueryable().FirstOrDefault(t => title.Trim().Equals(t.Title.Trim()));
        if (existTopic != null)
        {
            return ResultUtil.Success(existTopic.Id);
        }

        var topic = topicAddRequest.ToEntity();
        var userDto = UserUtil.GetUserFromHttpRequest(Request);
        topic.UserId = userDto.UserId;
        int topicId = await _topicService.CreateAsync(topic);
        return ResultUtil.Success(topicId);
    }

    [HttpPost]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] TopicEditRequest topicEditRequest)
        => ResultUtil.Success(await _topicService.EditTopicByRequest(topicEditRequest));


    [HttpDelete("{id}")]
    [Authorize(Roles = UserConstant.ADMIN_ROLE)]
    // Related Articles will not be deleted
    public async Task<ApiResponse<bool>> DeleteById(int id)
        => ResultUtil.Success(await _topicService.DeleteAsync(id));


}