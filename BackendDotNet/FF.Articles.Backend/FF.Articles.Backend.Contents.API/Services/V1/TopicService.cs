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
using FF.Articles.Backend.Contents.API.UnitOfWork;
using FF.Articles.Backend.Contents.API.Interfaces.Services;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
namespace FF.Articles.Backend.Contents.API.Services.V1;
public class TopicService : BaseService<Topic, ContentsDbContext>, ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleService _articleService;
    private readonly IIdentityRemoteService _identityRemoteService;
    private readonly IContentsUnitOfWork _contentsUnitOfWork;
    public TopicService(
        Func<string, ITopicRepository> topicRepository,
        Func<string, IArticleRepository> articleRepository,
        Func<string, IArticleService> articleService,
        Func<string, IIdentityRemoteService> identityRemoteService,
        IContentsUnitOfWork contentsUnitOfWork,
        ILogger<TopicService> logger
    )
    : base(topicRepository("v1"), logger)
    {
        _topicRepository = topicRepository("v1");
        _articleRepository = articleRepository("v1");
        _articleService = articleService("v1");
        _identityRemoteService = identityRemoteService("v1");
        _contentsUnitOfWork = contentsUnitOfWork;
    }
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
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var topic = await _topicRepository.GetByIdAsync(topicEditRequest.TopicId, true);
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
            await _topicRepository.UpdateModifiedAsync(topic);
        });
        return true;
    }
}

