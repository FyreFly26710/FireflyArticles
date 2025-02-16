using AutoMapper;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Common.Requests;
using FF.Articles.Backend.Common.Responses;
using FF.Articles.Backend.Common.Utils;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;
using FF.Articles.Backend.Contents.API.Models.Responses;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services;
using FF.Articles.Backend.Contents.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FF.Articles.Backend.Contents.API.Controllers;
[ApiController]
[Route("api/contents/topic")]
public class TopicController(ILogger<TopicController> _logger, IMapper _mapper,
    ITopicService _topicService,
    IArticleService _articleService,
    IIdentityRemoteService _identityRemoteService)
    : ControllerBase
{
    /// <summary>
    /// Get topic response by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get/{id}")]
    public async Task<ApiResponse<TopicResponse>> GetById(int id)
    {
        if (id <= 0)
            return ResultUtil.Error<TopicResponse>(ErrorCode.PARAMS_ERROR, "Invalid topic id");
        var topic = await _topicService.GetByIdAsync(id);
        if (topic == null)
            return ResultUtil.Error<TopicResponse>(ErrorCode.NOT_FOUND_ERROR, "Topic not found");
        var topicResponse = _mapper.Map<TopicResponse>(topic);
        topicResponse.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        List<Article> articles = _articleService.GetQueryable().Where(x => x.TopicId == id).OrderBy(x => x.SortNumber).ToList();
        var pageResponse = new PageResponse<ArticleResponse>();
        _mapper.Map(articles, pageResponse.Data);
        pageResponse.PageSize = 100;
        pageResponse.RecordCount = articles.Count;
        topicResponse.Articles = pageResponse;
        return ResultUtil.Success(topicResponse);
    }
    /// <summary>
    /// Get topics by page
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpPost("get-page")]
    public async Task<ApiResponse<PageResponse<TopicResponse>>> GetByPage(PageRequest pageRequest)
    {
        if (pageRequest == null || pageRequest.PageSize > 200)
            return ResultUtil.Error<PageResponse<TopicResponse>>(ErrorCode.PARAMS_ERROR, "Invalid page request");
        if (pageRequest.SortField == null)
        {
            pageRequest.SortField = "SortNumber";
        }
        var topics = await _topicService.GetAllAsync(pageRequest);
        var topicList = _mapper.Map<List<TopicResponse>>(topics.Data);
        foreach (var topic in topicList)
        {
            topic.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
            List<Article> articles = _articleService.GetQueryable().Where(x => x.TopicId == topic.TopicId).OrderBy(x => x.SortNumber).ToList();
            var pageResponse = new PageResponse<ArticleResponse>();
            _mapper.Map(articles, pageResponse.Data);
            pageResponse.PageSize = 100;
            pageResponse.RecordCount = articles.Count;
            topic.Articles = pageResponse;        }
        var res = new PageResponse<TopicResponse>()
        {
            Data = topicList,
            PageIndex = topics.PageIndex,
            PageSize = topics.PageSize,
            RecordCount = topics.RecordCount
        };
        return ResultUtil.Success(res);
    }




    #region DB Modification
    /// <summary>
    /// Add topic
    /// </summary>
    /// <param name="topicAddRequest"></param>
    /// <returns></returns>
    [HttpPost("add")]
    public async Task<ApiResponse<int>> AddTopic([FromBody] TopicAddRequest topicAddRequest)
    {
        if (topicAddRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);
        var topic = _mapper.Map<Topic>(topicAddRequest);
        var userDto = UserUtil.GetUserFromHttpRequest(Request);
        topic.UserId = userDto.UserId;
        int topicId = await _topicService.CreateAsync(topic);
        return ResultUtil.Success(topic.Id);
    }
    /// <summary>
    /// Edit topic
    /// </summary>
    /// <param name="topicEditRequest"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    public async Task<ApiResponse<int>> EditTopic([FromBody] TopicEditRequest topicEditRequest)
    {
        if (topicEditRequest == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR);

        var topic = await _topicService.GetByIdAsTrackingAsync(topicEditRequest.TopicId);
        if (topic == null)
            return ResultUtil.Error<int>(ErrorCode.PARAMS_ERROR, "Topic not found");

        if (topicEditRequest.IsHidden != 0) topic.IsHidden = topicEditRequest.IsHidden;
        if (!string.IsNullOrWhiteSpace(topicEditRequest.Title)) topic.Title = topicEditRequest.Title;
        if (!string.IsNullOrWhiteSpace(topicEditRequest.Content)) topic.Content = topicEditRequest.Content;
        if (!string.IsNullOrWhiteSpace(topicEditRequest.Abstraction)) topic.Abstraction = topicEditRequest.Abstraction;
        if (topicEditRequest.SortNumber > 0) topic.SortNumber = topicEditRequest.SortNumber;
        await _topicService.UpdateAsync(topic);

        return ResultUtil.Success(topic.Id);
    }
    /// <summary>
    /// Delete topic
    /// </summary>
    /// <param name="deleteByIdRequest"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    public async Task<ApiResponse<bool>> DeleteTopic([FromBody] DeleteByIdRequest deleteByIdRequest)
    {
        if (deleteByIdRequest.Id <= 0)
            return ResultUtil.Error<bool>(ErrorCode.PARAMS_ERROR, "Invalid topic id");
        await _topicService.DeleteAsync(deleteByIdRequest.Id);
        return ResultUtil.Success(true);
    }

    #endregion
}