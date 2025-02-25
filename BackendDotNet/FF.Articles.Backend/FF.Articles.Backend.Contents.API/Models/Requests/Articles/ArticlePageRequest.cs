using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles
{
    public class ArticlePageRequest:PageRequest
    {
        public bool IncludeUser { get; set; } = true;
        public bool IncludeSubArticles { get; set; } = true;
        public bool IncludeContent { get; set; } = true;
    }
}
