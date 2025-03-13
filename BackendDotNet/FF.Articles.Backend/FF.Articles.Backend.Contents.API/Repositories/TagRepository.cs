using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;

namespace FF.Articles.Backend.Contents.API.Repositories;
public class TagRepository : BaseRepository<Tag, ContentsDbContext>, ITagRepository
{
    public TagRepository(ContentsDbContext _context) : base(_context)
    {
    }
}
