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
using FF.Articles.Backend.Contents.API.Interfaces.Services.RemoteServices;
using Microsoft.EntityFrameworkCore;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
namespace FF.Articles.Backend.Contents.API.Services.V1;
public class TopicService : BaseService<Topic, ContentsDbContext>, ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IArticleService _articleService;
    private readonly IIdentityRemoteService _identityRemoteService;
    private readonly IContentsUnitOfWork _contentsUnitOfWork;
    public TopicService(
        ITopicRepository topicRepository,
        IArticleRepository articleRepository,
        Func<string, IArticleService> articleService,
        IIdentityRemoteService identityRemoteService,
        IContentsUnitOfWork contentsUnitOfWork,
        ILogger<TopicService> logger
    )
    : base(topicRepository, logger)
    {
        _topicRepository = topicRepository;
        _articleRepository = articleRepository;
        _articleService = articleService("v1");
        _identityRemoteService = identityRemoteService;
        _contentsUnitOfWork = contentsUnitOfWork;
    }
    public async Task<TopicDto> GetTopicDto(Topic topic) => await GetTopicDto(topic, new TopicQueryRequest());
    public async Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest)
    {
        var topicDto = new TopicDto();
        if (topicRequest.OnlyCategoryTopic)
        {
            topicDto = new TopicDto
            {
                TopicId = topic.Id,
                Title = topic.Title,
                Category = topic.Category,
            };
            return topicDto;
        }

        topicDto = topic.ToDto();
        if (topicRequest.IncludeUser) topicDto.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        if (topicRequest.IncludeArticles)
        {
            // Only get articles with ArticleType = Article
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
            await _topicRepository.UpdateAsync(topic);
        });
        return true;
    }
    public async Task<Topic?> GetTopicByTitle(string title)
    {
        return await _topicRepository.GetQueryable()
            .FirstOrDefaultAsync(t => t.Title.Trim().ToLower() == title.Trim().ToLower());
    }
    public override async Task<bool> DeleteAsync(long topicId)
    {
        await _contentsUnitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _articleRepository.SetTopicIdToZero(topicId);
            await _topicRepository.DeleteAsync(topicId);
        });
        return true;
    }
}

