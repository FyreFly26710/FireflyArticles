﻿namespace FF.Articles.Backend.Contents.API.Models.Requests.Articles;
public class ArticleAddRequest
{
    public string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Abstraction { get; set; } = string.Empty;
    public int TopicId { get; set; }
    public List<int> TagIds { get; set; }= new ();
    public int SortNumber { get; set; } = 1;
    public int IsHidden { get; set; }= 0;
}
