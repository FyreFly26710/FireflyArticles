namespace FF.Articles.Backend.Contents.API.Interfaces.Services;
public interface ITopicService : IBaseService<Topic>
{
    Task<TopicDto> GetTopicDto(Topic topic);
    Task<TopicDto> GetTopicDto(Topic topic, TopicQueryRequest topicRequest);
    Task<bool> EditTopicByRequest(TopicEditRequest topicEditRequest);
    Task<TopicDto?> GetTopicByTitleCategory(string title, string category);
}
