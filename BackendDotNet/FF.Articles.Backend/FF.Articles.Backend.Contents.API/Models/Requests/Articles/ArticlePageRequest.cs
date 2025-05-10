using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles
{
    public class ArticleQueryRequest : PageRequest
    {
        public long? ArticleId { get; set; } = null;

        public bool IncludeUser { get; set; } = false;

        public bool IncludeSubArticles { get; set; } = false;

        public bool IncludeContent { get; set; } = false;

        public bool DisplaySubArticles { get; set; } = false;

        public override string? SortField { get; set; } = "SortNumber";

        #region Search fields
        /// <summary>
        /// Search from title and abstract
        /// </summary>
        public string? Keyword { get; set; }
        public List<long>? TopicIds { get; set; }
        public List<long>? TagIds { get; set; }
        /// <summary>
        /// Enable Elasticsearch relevance sort
        /// </summary>
        public bool SortByRelevance { get; set; } = false;

        #endregion


    }
}
