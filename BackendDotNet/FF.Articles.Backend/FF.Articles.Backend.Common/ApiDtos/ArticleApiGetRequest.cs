namespace FF.Articles.Backend.Common.ApiDtos;

public class ArticleApiGetRequest
{
    public long ArticleId { get; set; }

    public bool IncludeUser { get; set; } = false;
    /// <summary>
    /// Include sub articles as children
    /// </summary>
    public bool IncludeSubArticles { get; set; } = false;

    public bool IncludeContent { get; set; } = false;

    /// <summary>
    /// Display sub articles as parent articles (flat display)
    /// </summary>
    public bool DisplaySubArticles { get; set; } = false;
    /// <summary>
    /// Display topic articles
    /// </summary>
    public bool DisplayTopicArticles { get; set; } = false;

}

