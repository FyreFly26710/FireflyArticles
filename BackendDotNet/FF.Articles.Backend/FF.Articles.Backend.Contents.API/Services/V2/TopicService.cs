using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Contents.API.Constants;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using FF.Articles.Backend.Contents.API.MapperExtensions.Topics;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.Services.V2;

public class TopicService : RedisService<Topic>, ITopicService
{
    private readonly ITopicRedisRepository _topicRedisRepository;
    private readonly IArticleRedisRepository _articleRedisRepository;
    private readonly IIdentityRemoteService _identityRemoteService;
    private readonly IArticleService _articleService;
    public TopicService(
        ITopicRedisRepository topicRedisRepository,
        IArticleRedisRepository articleRedisRepository,
        IIdentityRemoteService identityRemoteService,
        Func<string, IArticleService> articleService,
        ILogger<TopicService> logger)
        : base(topicRedisRepository, logger)
    {
        _topicRedisRepository = topicRedisRepository;
        _articleRedisRepository = articleRedisRepository;
        _identityRemoteService = identityRemoteService;
        _articleService = articleService("v2");
    }

    public async Task<TopicDto> GetTopicDto(Topic topic) => await GetTopicDto(topic, new TopicQueryRequest());
    public async Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest)
    {
        var topicDto = topic.ToDto();
        if (topicRequest.IncludeUser) topicDto.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        if (topicRequest.IncludeArticles)
        {
            // Only get articles with ArticleType = Article
            List<Article> articles = (await _articleRedisRepository.GetArticlesByTopicIdAsync(topicDto.TopicId))
                .Where(x => x.ArticleType == ArticleTypes.Article)
                .OrderBy(x => x.SortNumber)
                .ToList();
            topicDto.Articles = await _articleService.GetArticleDtos(articles, topicRequest.ToArticlePageRequest());
        }
        return topicDto;
    }
    public async Task<bool> EditTopicByRequest(TopicEditRequest topicEditRequest)
    {
        var topic = await _topicRedisRepository.GetByIdAsync(topicEditRequest.TopicId);
        if (topic == null)
            throw new ApiException(ErrorCode.NOT_FOUND_ERROR, "Topic not found");
        if (topicEditRequest.IsHidden != null && topic.IsHidden != topicEditRequest.IsHidden)
            topic.IsHidden = (int)topicEditRequest.IsHidden;
        if (topicEditRequest.Title != null && topic.Title != topicEditRequest.Title)
            topic.Title = topicEditRequest.Title;
        if (topicEditRequest.Abstract != null && topic.Abstract != topicEditRequest.Abstract)
            topic.Abstract = topicEditRequest.Abstract;
        if (topicEditRequest.Content != null && topic.Content != topicEditRequest.Content)
            topic.Content = topicEditRequest.Content;
        if (topicEditRequest.Category != null && topic.Category != topicEditRequest.Category)
            topic.Category = topicEditRequest.Category;
        if (topicEditRequest.TopicImage != null && topic.TopicImage != topicEditRequest.TopicImage)
            topic.TopicImage = topicEditRequest.TopicImage;
        if (topicEditRequest.SortNumber != null && topic.SortNumber != topicEditRequest.SortNumber)
            topic.SortNumber = (int)topicEditRequest.SortNumber;
        await _topicRedisRepository.UpdateAsync(topic);
        return true;
    }
    public async Task<Topic?> GetTopicByTitle(string title)
    {
        var topics = (await _topicRedisRepository.GetAllAsync())
            .FirstOrDefault(t => t.Title.Trim().ToLower() == title.Trim().ToLower());
        return topics;
    }
    public async Task<bool> DeleteAsync(long topicId)
    {
        await _articleRedisRepository.SetTopicIdToZero(topicId);
        await _topicRedisRepository.DeleteAsync(topicId);
        return true;
    }
}
