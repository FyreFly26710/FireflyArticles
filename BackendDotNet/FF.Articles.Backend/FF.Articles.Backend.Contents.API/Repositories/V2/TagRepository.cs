using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;
public class TagRepository : BaseRepository<Tag, ContentsDbContext>, ITagRepository
{
    public TagRepository(ContentsDbContext _context) : base(_context)
    {
    }
}
