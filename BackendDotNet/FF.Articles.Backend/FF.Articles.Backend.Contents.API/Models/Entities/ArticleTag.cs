namespace FF.Articles.Backend.Contents.API.Models.Entities;
/// <summary>
/// Ignore BaseEntity optional columns
/// </summary>
public class ArticleTag : BaseEntity
{
    public long ArticleId { get; set; }
    public long TagId { get; set; }
}
