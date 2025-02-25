using AutoMapper;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Common.Exceptions;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;
using FF.Articles.Backend.Contents.API.RemoteServices.Interfaces;
using FF.Articles.Backend.Contents.API.Services.Interfaces;

namespace FF.Articles.Backend.Contents.API.Services;
public class TopicService(ContentsDbContext _context, ILogger<TopicService> _logger, IMapper _mapper,
    IArticleService _articleService,
    IIdentityRemoteService _identityRemoteService)
: BaseService<Topic, ContentsDbContext>(_context, _logger), ITopicService
{
    public async Task<TopicDto> GetTopicDto(Topic topic) => await GetTopicDto(topic, new TopicPageRequest());
    public async Task<TopicDto> GetTopicDto(Topic topic, TopicPageRequest topicRequest)
    {
        var topicDto = _mapper.Map<TopicDto>(topic);
        if (topicRequest.IncludeUser) topicDto.User = await _identityRemoteService.GetUserByIdAsync(topic.UserId);
        if (topicRequest.IncludeArticles)
        {
            List<Article> articles = _context.Set<Article>().AsQueryable()
                .Where(x => x.TopicId == topicDto.TopicId)
                .OrderBy(x => x.SortNumber)
                .ToList();
            //var articleRequest = _mapper.Map<ArticlePageRequest>(topicRequest);
            var articleRequest = new ArticlePageRequest();
            articleRequest.IncludeUser = topicRequest.IncludeUser;
            articleRequest.IncludeSubArticles = topicRequest.IncludeSubArticles;
            articleRequest.IncludeContent = topicRequest.IncludeContent;
            var articleDtos = await _articleService.GetArticleDtos(articles, articleRequest);
            topicDto.Articles = _mapper.Map<List<ArticleMiniDto>>(articleDtos);
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
        if (topicEditRequest.Abstraction!=null) topic.Abstraction = topicEditRequest.Abstraction;
        if (topicEditRequest.Category!=null) topic.Category = topicEditRequest.Category;
        if (topicEditRequest.TopicImage!=null) topic.TopicImage = topicEditRequest.TopicImage;
        if (topicEditRequest.SortNumber!=null) topic.SortNumber = (int)topicEditRequest.SortNumber;
        await this.UpdateAsync(topic);

        return true;
    }
}

