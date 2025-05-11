namespace FF.Articles.Backend.Contents.API.MapperExtensions;
public static class TopicExtensions
{
    public static TopicDto ToDto(this Topic topic)
    {
        var topicDto = new TopicDto
        {
            TopicId = topic.Id,
            Title = topic.Title,
            Abstract = topic.Abstract,
            Category = topic.Category,
            TopicImage = topic.TopicImage ?? "",
            UserId = topic.UserId,
            SortNumber = topic.SortNumber,
            IsHidden = topic.IsHidden
        };
        return topicDto;
    }
    public static Topic ToEntity(this TopicAddRequest topicAddRequest)
    {
        var topic = new Topic
        {
            Title = topicAddRequest.Title,
            Abstract = topicAddRequest.Abstract,
            Category = topicAddRequest.Category,
            TopicImage = topicAddRequest.TopicImage,
            //UserId = topicAddRequest.UserId,
            SortNumber = topicAddRequest.SortNumber,
            IsHidden = topicAddRequest.IsHidden
        };
        return topic;
    }
    public static ArticleQueryRequest ToArticlePageRequest(this TopicQueryRequest topicPageRequest)
    {
        return new ArticleQueryRequest
        {
            IncludeUser = topicPageRequest.IncludeUser,
            IncludeSubArticles = topicPageRequest.IncludeSubArticles,
            IncludeContent = topicPageRequest.IncludeContent,
            DisplaySubArticles = false,
        };
    }
}