﻿namespace FF.Articles.Backend.Contents.API.Services;
public class TopicService(
    ITopicRepository _topicRepository,
    IArticleRepository _articleRepository,
    IArticleService _articleService,
    IIdentityRemoteService _identityRemoteService,
    IContentsUnitOfWork _contentsUnitOfWork,
    ILogger<TopicService> _logger
) : BaseService<Topic, ContentsDbContext>(_topicRepository, _logger), ITopicService
{
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
            //if (topicEditRequest.Content != null && topic.Content != topicEditRequest.Content)
            //    topic.Content = topicEditRequest.Content;
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
    public async Task<TopicDto?> GetTopicByTitleCategory(string title, string category)
    {
        var topic = await _topicRepository.GetQueryable()
            .FirstOrDefaultAsync(t => t.Title.Trim().ToLower() == title.Trim().ToLower()
                 && t.Category.Trim().ToLower() == category.Trim().ToLower());
        if (topic == null) return null;
        var request = new TopicQueryRequest
        {
            IncludeArticles = true,
        };
        var topicDto = await GetTopicDto(topic, request);
        return topicDto;
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
    public override async Task<long> CreateAsync(Topic entity)
    {
        var id = await _topicRepository.CreateAsync(entity);
        var article = new Article
        {
            Id = id,
            Title = entity.Title,
            Abstract = entity.Abstract,
            Content = "",
            ArticleType = ArticleTypes.TopicArticle,
            ParentArticleId = null,
            UserId = entity.UserId,
            TopicId = id,
            SortNumber = 0,
            IsHidden = 0,
            CreateTime = DateTime.UtcNow,
            UpdateTime = DateTime.UtcNow
        };
        await _articleRepository.CreateAsync(article);
        await _topicRepository.SaveChangesAsync();
        return id;
    }
}

