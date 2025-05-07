using FF.Articles.Backend.Common.Responses;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles
{
    public class ArticleQueryRequest : PageRequest
    {
        /// <summary>
        /// Optional: Article Id
        /// </summary>
        public long? ArticleId { get; set; } = null;

        /// <summary>
        /// Include user
        /// </summary>
        public bool IncludeUser { get; set; } = false;

        /// <summary>
        /// Inlcude sub articles as children
        /// </summary>
        public bool IncludeSubArticles { get; set; } = false;

        /// <summary>
        /// Include content
        /// </summary>
        public bool IncludeContent { get; set; } = false;

        /// <summary>
        /// Dispaly sub articles as articles
        /// </summary>
        public bool DisplaySubArticles { get; set; } = false;

        /// <summary>
        /// Sort field. Default: SortNumber
        /// </summary>
        public override string? SortField { get; set; } = "SortNumber";

        #region Search fields
        /// <summary>
        /// Keyword
        /// </summary>
        public string? Keyword { get; set; }
        /// <summary>
        /// Topic Ids
        /// </summary>
        public List<long>? TopicIds { get; set; }
        /// <summary>
        /// Tag Ids
        /// </summary>
        public List<long>? TagIds { get; set; }

        #endregion


    }
}
