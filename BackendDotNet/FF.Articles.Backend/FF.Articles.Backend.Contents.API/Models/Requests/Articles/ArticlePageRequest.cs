using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles
{
    public class ArticlePageRequest:PageRequest
    {
        public int? ArticleId { get; set; } = null;
        public bool IncludeUser { get; set; } = true;
        /// <summary>
        /// Inlcude sub articles as children
        /// </summary>
        public bool IncludeSubArticles { get; set; } = true;
        public bool IncludeContent { get; set; } = true;
        /// <summary>
        /// Dispaly sub articles as articles
        /// </summary>
        public bool DisplaySubArticles {get; set; } = false;

        #region Search fields
        public string? Keyword {get; set; }
        public List<int>? TopicIds {get; set; }
        public List<int>? TagIds {get; set;}

        #endregion


    }
}
