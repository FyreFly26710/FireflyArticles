using FF.Articles.Backend.Contents.API.Models.Requests.Articles;
using FF.Articles.Backend.Contents.API.Models.Requests.Topics;

namespace FF.Articles.Backend.Contents.API.MapperExtensions.Topics;
public static class TopicPageRequestExtensions
{
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
