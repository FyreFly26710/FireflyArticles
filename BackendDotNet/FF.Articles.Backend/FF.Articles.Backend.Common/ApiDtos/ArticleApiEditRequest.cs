using System;

namespace FF.Articles.Backend.Common.ApiDtos;

public class ArticleApiEditRequest
{
    public long ArticleId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Abstract { get; set; }
    public string? ArticleType { get; set; }
    public long? ParentArticleId { get; set; }
    public long? TopicId { get; set; }
    public List<string>? TagId { get; set; }
    public int? SortNumber { get; set; }
    public int? IsHidden { get; set; }
}
