using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TopicService(ContentsDbContext _context, ILogger<TopicService> _logger, IMapper _mapper,
    IArticleService _articleService,
    IIdentityRemoteService _identityRemoteService)
: CacheService<Topic, ContentsDbContext>(_context, _logger), ITopicService
{
    public async Task<TopicDto> GetTopicDto(Topic topic, bool includeUser = true, bool IncludeArticles = true)
    {
        var topicDto = _mapper.Map<TopicDto>(topic);
        if (includeUser) topicDto.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        if (IncludeArticles)
        {
            List<Article> articles = _context.Set<Article>().AsQueryable()
                .Where(x => x.TopicId == topicDto.TopicId)
                .OrderBy(x => x.SortNumber)
                .ToList();

            topicDto.Articles = await _articleService.GetArticleDtos(articles, includeUser);
        }
        return topicDto;
    }
    public async Task<bool> EditArticleByRequest(TopicEditRequest topicEditRequest)
    {
        var topic = await this.GetByIdAsTrackingAsync(topicEditRequest.TopicId);
        if (topic == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Topic not found");

        if (topicEditRequest.IsHidden != null) topic.IsHidden = (int)topicEditRequest.IsHidden;
        if (topicEditRequest.Title!=null) topic.Title = topicEditRequest.Title;
        if (topicEditRequest.Content!=null) topic.Content = topicEditRequest.Content;
        if (topicEditRequest.Abstraction!=null) topic.Abstraction = topicEditRequest.Abstraction;
        if (topicEditRequest.SortNumber!=null) topic.SortNumber = (int)topicEditRequest.SortNumber;
        await this.UpdateAsync(topic);

        return true;
    }
}

