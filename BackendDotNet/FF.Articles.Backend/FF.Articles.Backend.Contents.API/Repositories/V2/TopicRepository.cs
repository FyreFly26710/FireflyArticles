using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V2;
public class TopicRepository : BaseRepository<Topic, ContentsDbContext>, ITopicRepository
{
    public TopicRepository(ContentsDbContext _context) : base(_context)
    {
    }

}
