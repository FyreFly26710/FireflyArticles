using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Contents.API.Models.Entities;
/// <summary>
/// Ignore BaseEntity optional columns
/// </summary>
public class ArticleTag : BaseEntity
{
    public int ArticleId { get; set; }
    public int TagId { get; set; }
}
