using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;
using FF.Articles.Backend.Contents.API.Repositories.Interfaces;

namespace FF.Articles.Backend.Contents.API.Repositories;
public class TopicRepository : BaseRepository<Topic, ContentsDbContext>, ITopicRepository
{
    public TopicRepository(ContentsDbContext _context) : base(_context)
    {
    }
}
