﻿
using FF.Articles.Backend.Contents.API.Constants;

namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleAddRequest
{
    public string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    /// <summary>
    ///  Article = 1, SubArticle = 2, TopicArticle = 3
    /// </summary>
    public string ArticleType { get; set; } = ArticleTypes.Article; 
    public int? ParentArticleId { get; set; }
    public int TopicId { get; set; }
    public List<int> TagIds { get; set; }= new ();
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; }= 0;
}
