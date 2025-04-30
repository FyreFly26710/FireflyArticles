using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Topics
{
    public class TopicQueryRequest : PageRequest
    {
        public long? TopicId { get; set; } = null;
        public bool IncludeUser { get; set; } = false;
        public bool IncludeArticles { get; set; } = false;
        public bool IncludeSubArticles { get; set; } = false;
        public bool IncludeContent { get; set; } = false;
        public bool OnlyCategoryTopic { get; set; } = false;
        public override string? SortField { get; set; } = "SortNumber";
    }
}
