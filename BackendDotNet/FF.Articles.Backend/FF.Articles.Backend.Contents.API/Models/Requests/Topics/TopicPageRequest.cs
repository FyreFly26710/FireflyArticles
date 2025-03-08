using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics
{
    public class TopicPageRequest : PageRequest
    {
        public int? TopicId { get; set; } = null;
        public bool IncludeUser { get; set; } = true;
        public bool IncludeArticles { get; set; } = true;
        public bool IncludeSubArticles { get; set; } = true;
        public bool IncludeContent { get; set; } = false;
    }
}
