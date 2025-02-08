using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Contents.API.Models.Entities;
public class ArticleTag : BaseEntity
{
    public long ArticleId { get; set; }
    public long TagId { get; set; }
}
