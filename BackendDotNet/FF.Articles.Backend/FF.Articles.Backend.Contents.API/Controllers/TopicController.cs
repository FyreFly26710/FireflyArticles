using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/topics")]
public class TopicController(ILogger<TopicController> _logger, IMapper _mapper,
    ITopicService _topicService,
    IArticleService _articleService,
    IIdentityRemoteService _identityRemoteService)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ApiResponse<TopicDto>> GetById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<TopicDto>(ErrorCode.PARAMS_ERROR, "Invalid topic id");
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null)
            return ResultUtil.Error<TopicDto>(ErrorCode.NOT_FOUND_ERROR, "Topic not found");
        var topicResponse = _mapper.Map<TopicDto>(topic);
        topicResponse.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        List<Article> articles = _articleService.GetQueryable().Where(x => x.TopicId == topicResponse.TopicId).OrderBy(x => x.SortNumber).ToList();
        topicResponse.Articles = _mapper.Map<List<ArticleDto>>(articles);
        return ResultUtil.Success(topicResponse);
    }
    [HttpGet]
    public async Task<ApiResponse<Paged<TopicDto>>> GetByPage([FromQuery] TopicPageRequest pageRequest)
    {
        if (pageRequest == null || pageRequest.PageSize > 200)
            return ResultUtil.Error<Paged<TopicDto>>(ErrorCode.PARAMS_ERROR, "Invalid page request");
        if (pageRequest.SortField == null)
        {
            pageRequest.SortField = "SortNumber";
        }
        Paged<Topic> topics = await _topicService.GetAllAsync(pageRequest);
        Paged<TopicDto> res = new(topics.GetPageInfo());
        List <TopicDto> topicList = _mapper.Map<List<TopicDto>>(topics.Data);
        foreach (var topic in topicList)
        {
            if (pageRequest.IncludeUser) topic.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
            if (pageRequest.IncludeArticles)
            {
                List<Article> articles = _articleService.GetQueryable().Where(x => x.TopicId == topic.TopicId).OrderBy(x => x.SortNumber).ToList();
                topic.Articles = _mapper.Map<List<ArticleDto>>(articles);
            }
        }
        res.Data = topicList;
        return ResultUtil.Success(res);
    }

    [HttpPut]
    public async Task<ApiResponse<int>> AddByRequest([FromBody] TopicAddRequest topicAddRequest)
    {
        if (topicAddRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);
        var topic = _mapper.Map<Topic>(topicAddRequest);
        var userDto = UserUtil.GetUserFromHttpRequest(Request);
        topic.UserId = userDto.UserId;
        int topicId = await _topicService.CreateAsync(topic);
        return ResultUtil.Success(topic.Id);
    }

    [HttpPost]
    public async Task<ApiResponse<bool>> EditByRequest([FromBody] TopicEditRequest topicEditRequest)
    {
        if (topicEditRequest == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR);

        var topic = await _topicService.GetByIdAsTrackingAsync(topicEditRequest.TopicId);
        if (topic == null)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Topic not found");

        if (topicEditRequest.IsHidden != 0) topic.IsHidden = topicEditRequest.IsHidden;
        if (!string.IsNullOrWhiteSpace(topicEditRequest.Title)) topic.Title = topicEditRequest.Title;
        if (!string.IsNullOrWhiteSpace(topicEditRequest.Content)) topic.Content = topicEditRequest.Content;
        if (!string.IsNullOrWhiteSpace(topicEditRequest.Abstraction)) topic.Abstraction = topicEditRequest.Abstraction;
        if (topicEditRequest.SortNumber > 0) topic.SortNumber = topicEditRequest.SortNumber;
        await _topicService.UpdateAsync(topic);

        return ResultUtil.Success(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid topic id");
        await _topicService.DeleteAsync(id);
        return ResultUtil.Success(true);
    }

}