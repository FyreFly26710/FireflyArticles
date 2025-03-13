using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.MapperExtensions.Topics;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.MapperExtensions.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TopicService(ITopicRepository _topicRepository, ILogger<TopicService> _logger,
    IArticleRepository _articleRepository,
    IArticleService _articleService,
    IIdentityRemoteService _identityRemoteService)
: BaseService<Topic, ContentsDbContext>(_topicRepository, _logger), ITopicService
{
    public async Task<TopicDto> GetTopicDto(Topic topic) => await GetTopicDto(topic, new TopicQueryRequest());
    public async Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest)
    {
        var topicDto = topic.ToDto();
        if (topicRequest.IncludeUser) topicDto.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        if (topicRequest.IncludeArticles)
        {
            List<Article> articles = _articleRepository.GetQueryable()
                .Where(x => x.TopicId == topicDto.TopicId && x.ArticleType == ArticleTypes.Article)
                .OrderBy(x => x.SortNumber)
                .ToList();
            topicDto.Articles = await _articleService.GetArticleDtos(articles, topicRequest.ToArticlePageRequest());
        }
        return topicDto;
    }
    public async Task<bool> EditTopicByRequest(TopicEditRequest topicEditRequest)
    {
        var topic = await this.GetByIdAsTrackingAsync(topicEditRequest.TopicId);
        if (topic == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Topic not found");

        if (topicEditRequest.IsHidden != null && topic.IsHidden != topicEditRequest.IsHidden)
            topic.IsHidden = (int)topicEditRequest.IsHidden;
        if (topicEditRequest.Title != null && topic.Title != topicEditRequest.Title)
            topic.Title = topicEditRequest.Title;
        if (topicEditRequest.Abstraction != null && topic.Abstraction != topicEditRequest.Abstraction)
            topic.Abstraction = topicEditRequest.Abstraction;
        if (topicEditRequest.Content != null && topic.Content != topicEditRequest.Content)
            topic.Content = topicEditRequest.Content;
        if (topicEditRequest.Category != null && topic.Category != topicEditRequest.Category)
            topic.Category = topicEditRequest.Category;
        if (topicEditRequest.TopicImage != null && topic.TopicImage != topicEditRequest.TopicImage)
            topic.TopicImage = topicEditRequest.TopicImage;
        if (topicEditRequest.SortNumber != null && topic.SortNumber != topicEditRequest.SortNumber)
            topic.SortNumber = (int)topicEditRequest.SortNumber;
        await this.UpdateAsync(topic);

        return true;
    }
}

