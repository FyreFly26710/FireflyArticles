using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics
{
    public class TopicPageRequest:PageRequest
    {
        public bool IncludeUser { get; set; } = true;
        public bool IncludeArticles { get; set; } = true;
    }
}
