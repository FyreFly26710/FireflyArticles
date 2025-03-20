using System;

namespace FF.Articles.Backend.Common.ApiDtos;

public class ArticleApiAddRequest
{
    public string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    /// <summary>
    ///  Article = 1, SubArticle = 2, TopicArticle = 3
    /// </summary>
    public string ArticleType { get; set; }
    public int? ParentArticleId { get; set; }
    public int TopicId { get; set; }
    public List<int> TagIds { get; set; } = new();
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; } = 0;
}

