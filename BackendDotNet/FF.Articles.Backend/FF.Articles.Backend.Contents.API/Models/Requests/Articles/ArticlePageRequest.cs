using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles
{
    public class ArticlePageRequest:PageRequest
    {
        public bool IncludeUser { get; set; } = true;
    }
}
