using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Repositories;
public class ArticleTagRepository : BaseRepository<ArticleTag, ContentsDbContext>, IArticleTagRepository
{
    public ArticleTagRepository(ContentsDbContext _context) : base(_context)
    {
    }

}
